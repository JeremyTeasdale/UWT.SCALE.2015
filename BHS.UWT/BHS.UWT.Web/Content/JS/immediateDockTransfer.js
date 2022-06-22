//Modifications History
// Task     | By	    | Date          | Modification Description
// ---------|-----------|---------------|-------------------------
// 36833    | TDA       | 09/29/08      | Created
// 37680    | DSK       | 10/10/08      | Added logic to get the Uri for GET and POST
// 58804	| JAG		| 09/22/09		| Added support to transfer a multi order pallet

var _httpRequest = new HttpRequest();
_httpRequest.returnURL = "ImmediateDockTransfer.aspx";
var _shippingEntityId = "";
var _carrierObj;
var _contIdObj;
var _custNameObj;
var _destLocObj;
var _shipIdObj;
var _shipLoadNumObj;
var _shipToObj;

var _btnTransferObj;
var _uriGetCont;
var _uriTransferCont;
var _container;
var _uriTransferMop;
var _uriGetMop;
var _multiOrderPallet;
var _processStartTime;

$(document).ready(function() {

    getObjects();
    getURIValues();
    registerEvents();
    focusContainerId();
    _btnTransferObj.disabled = true;
    _processStartTime = "\/Date(" + document.getElementById("ProcessStartTime").value + ")\/";
});

function focusContainerId()
{
	_contIdObj.focus();
	_contIdObj.select();
}

function focusDestinationLocation()
{
	_destLocObj.focus();
	_destLocObj.select();
}

function formatMultipleDisplayField(list) {

	if (list != null && list.length > 0){
		
		var listLength = list.length;

		if (listLength == 1)
			return list[0];

		return manh.ils.utilities.valueFormatter.replaceVariableData(
			document.getElementById("MultipleEntriesText").value, [list.length]);		 
	}
}

function getContainer()
{
	var httpResponse = _httpRequest.performGet(_uriGetCont
		+ "?warehouse=" + manh.ils.clientSession.warehouse
		+ "&containerId=" + _contIdObj.value);
	
	if (httpResponse == null)
		return false;

	return this.handleGetContainer(httpResponse);
}

function getMultiOrderPallet() {

	var uri = manh.ils.httpClient.applyUriTemplateParameter(_uriGetMop, "mopId", _contIdObj.value);
	httpResponse = _httpRequest.performGet(uri);

	if (httpResponse == null)
		return false;

	handleGetMultiOrderPallet(httpResponse);
}

function getObjects() {
	_carrierObj = document.getElementById("carrierValue");
	_contIdObj = document.getElementById("txtBoxContainerId");
	_custNameObj = document.getElementById("custNameValue");
	_destLocObj = document.getElementById("txtBoxDestLoc");
	_shipIdObj = document.getElementById("shipIdValue");
	_shipLoadNumObj = document.getElementById("shipLoadNumValue");
	_shipToObj = document.getElementById("shipToValue");
	_btnTransferObj = document.getElementById("buttonSubmit");
}

function getShippingEntity() {

	var isContainer = getContainer();
	if (!isContainer) {
	    getMultiOrderPallet();
	}
	// at this point we have a valid container, go get the default location for it
	// added BHS
	else {

	    var parms = {
	        containerId: $('#txtBoxContainerId').val()
	    };


	    $.ajax({
	        type: "POST",
	        url: "/RF/BHSGetDockTransferDTO.asmx/GetDTO",
	        data: JSON.stringify(parms),
	        contentType: "application/json; charset=utf-8",
	        dataType: "json",
	        success: function(result) {

	            $('#containersValue').text(result.d.ContainerCount + ' of ' + result.d.ContainerCountTotal);

	            var loc = $('#txtBoxDestLoc');
	            loc.val(result.d.DockLocation);

	            if (result.d.DockLocation.length > 0) {
	                loc.attr('disabled', true)
	                $('#buttonSubmit').attr('disabled', false);
	                $('#txtBoxCheckDigit').focus();
	            }
	        },
	        error: function(xhr, err) {

	            alert('Error retrieving location');
	            //alert("readyState: "+xhr.readyState+"\nstatus: "+xhr.status);
	            //alert(xhr.statusText); 
	            //alert("responseText: "+xhr.responseText); 
	        }
	    });



	}

	// end added BHS

}

function getURIValues()
{
	_uriGetCont = document.getElementById("ShippingContainerUri").value;
	_uriGetMop = document.getElementById("MultiOrderPalletUri").value;
}

function handleErrorMessage(errorMessage) {
	focusContainerId();
	manh.ils.form.handleError(errorMessage);
}

function handleGetContainer(httpResponse)
{
	if (httpResponse.statusCode == 200)
	{
		_container = httpResponse.responseObject;
		_container.accessTime = _processStartTime;

		if (jQuery.trim(_destLocObj.value).length == 0
			&& _container.dockDoor !== null)
			_destLocObj.value = _container.dockDoor;
		if (_container.carrier !== null)
			_carrierObj.innerHTML = _container.carrier;
		if (_container.customer !== null)
			_custNameObj.innerHTML = _container.customer;
		_shipIdObj.innerHTML = _container.shipmentId;
		if (_container.shippingLoadNum !== 0)
			_shipLoadNumObj.innerHTML = _container.shippingLoadNum;
		_shipToObj.innerHTML = _container.shipTo;
		_uriTransferCont = _container.immediateDockTransfer;

		return true;
	}
	else 
	{
		
		_container = null;

		if (httpResponse.responseObject.ErrorCode != "MSG_CONT02") {
			handleErrorMessage(httpResponse.responseObject.Message);
			return true;
		}

		return false;
	}
}

function handleGetMultiOrderPallet(httpResponse) {



    if (httpResponse.statusCode == 200) {






        _multiOrderPallet = httpResponse.responseObject;

        if (jQuery.trim(_destLocObj.value).length == 0
			&& _multiOrderPallet.location !== null)
            _destLocObj.value = _multiOrderPallet.location;

        if (_multiOrderPallet.shippingLoadNum !== 0)
            _shipLoadNumObj.innerHTML = _multiOrderPallet.shippingLoadNum;

        if (_multiOrderPallet.carrier !== null)
            _carrierObj.innerHTML = _multiOrderPallet.carrier;

        if (_multiOrderPallet.customers !== null)
            _custNameObj.innerHTML = formatMultipleDisplayField(_multiOrderPallet.customers);

        if (_multiOrderPallet.shipmentIds !== null)
            _shipIdObj.innerHTML = formatMultipleDisplayField(_multiOrderPallet.shipmentIds);

        if (_multiOrderPallet.shipTos !== null)
            _shipToObj.innerHTML = formatMultipleDisplayField(_multiOrderPallet.shipTos);

        _uriTransferMop = manh.ils.utilities.linkRetrieval.getLinkByName(_multiOrderPallet.links, "transferMultiOrderPalletUri");
    }

    else {


        _multiOrderPallet = null;

        if (httpResponse.statusCode == 404)
            handleErrorMessage(document.getElementById("ContainerNotExist").value)

        else
            handleErrorMessage(httpResponse.responseObject.Message);
    }
}

function handleTransferContainer(httpResponse)
{
	if (httpResponse.statusCode == 200)
	{
		var msg = manh.ils.utilities.valueFormatter.replaceVariableData(
			document.getElementById("SuccessMsg").value, [_contIdObj.value]);
		manh.ils.utilities.statusBox.show(msg);
		reInitialize();
	}
	else
	{
		var error = httpResponse.responseObject;
		if (error.ErrorType == manh.ils.errorType.toLocation)
			focusDestinationLocation();
		else
			focusContainerId();
		manh.ils.form.handleError(error.Message);
	}
}

function onCancel()
{
	manh.ils.form.redirect("SignonMenuRF.aspx");
}

function onContainerIdChanged()
{
	resetContainerDataFields();

	var previousContainerId = _shippingEntityId;
	_shippingEntityId = _contIdObj.value;

	if (jQuery.trim(_shippingEntityId).length == 0
		|| previousContainerId == _shippingEntityId)
		return;

	getShippingEntity();
}

function onEnteredDataPossiblyChanged()
{
	setTransferButtonAccess();
}

function onSubmit()
{
    try {

        _btnTransferObj.disabled = true;
        manh.ils.form._upperCaseFields();
        if (validateCont()
	     && validateDestLoc()
		 && validateCheckDigit()) {

            $('#txtBoxDestLoc').attr('disabled', false);

            transferShpCont();
        }
    }
    finally {
        setTransferButtonAccess();
    }

	return false;
}

function validateCheckDigit() {

    // validate check digit
    
    var cd = $('#txtBoxCheckDigit');

    var parms = {
        userName: $('#BHSUserName').val(),
        environment: $('#BHSEnvironment').val(),
        doc: $('#txtBoxDestLoc').val(),
        checkDigit: cd.val()
    };

    var isValid = null;

    $.ajax({
        type: "POST",
        url: "/RF/BHSVerifiyLocationCheckDigit.asmx/IsValid",
        data: JSON.stringify(parms),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function(data) {

            isValid = data.d;

            if (!isValid) {

                alert('Invalid Check Digit');

                cd.focus();
                cd.select();
            }
        },
        error: function(xhr, err) {

            alert("readyState: " + xhr.readyState + "\nstatus: " + xhr.status);
            alert(xhr.statusText);
            alert("responseText: " + xhr.responseText);
        }
    });

    return isValid;	
}

function registerEvents()
{
	manh.ils.form.registerEvent("buttonSubmit", 'click', onSubmit);
	manh.ils.form.registerEvent("buttonCancel", 'click', onCancel);
	manh.ils.form.registerEvent("txtBoxContainerId", 'change', onContainerIdChanged);
        //manh.ils.form.registerEvent("txtBoxContainerId", 'blur', onContainerIdChanged);
	manh.ils.form.registerEvent("txtBoxContainerId", 'mouseup', onEnteredDataPossiblyChanged);
	manh.ils.form.registerEvent("txtBoxContainerId", 'keyup', onEnteredDataPossiblyChanged);
	manh.ils.form.registerEvent("txtBoxDestLoc", 'change', onEnteredDataPossiblyChanged);
	manh.ils.form.registerEvent("txtBoxDestLoc", 'mouseup', onEnteredDataPossiblyChanged);
	manh.ils.form.registerEvent("txtBoxDestLoc", 'keyup', onEnteredDataPossiblyChanged);
}

function reInitialize()
{
	_contIdObj.value = "";
	_shippingEntityId = "";
	_container = null;
	_multiOrderPallet = null;
	_destLocObj.value = "";
	resetContainerDataFields();
	focusContainerId();
}

function resetContainerDataFields() {

    // added BHS
    $('#txtBoxCheckDigit').val('');
	$('#txtBoxDestLoc').val('').attr('disabled', false);
	_carrierObj.innerHTML = "";
	_custNameObj.innerHTML = "";
	_shipIdObj.innerHTML = "";
	_shipLoadNumObj.innerHTML = "";
	_shipToObj.innerHTML = "";
	$('#containersValue').text('');
}

function setTransferButtonAccess()
{
	_btnTransferObj.disabled =
		(_container === null && _multiOrderPallet === null)
		|| jQuery.trim(_contIdObj.value).length == 0
		|| jQuery.trim(_destLocObj.value).length == 0
}

function transferShpCont() {

	var uri = "";
	var httpResponse = null;

	if (_container != null) {
		uri = manh.ils.httpClient.applyUriTemplateParameter(_uriTransferCont, "location", _destLocObj.value);
		httpResponse = _httpRequest.performPost(uri, _container);
	}
	else {
		uri = manh.ils.httpClient.applyUriTemplateParameter(_uriTransferMop, "location", _destLocObj.value);
		httpResponse = _httpRequest.performPost(uri, _multiOrderPallet);
	}
	
	if (httpResponse == null)
		return false;
	handleTransferContainer(httpResponse)
}

function validateCont()
{
	if (jQuery.trim(_contIdObj.value).length == 0)
	{
		focusContainerId();
		manh.ils.form.handleError(document.getElementById("ContErrorMsg").value);
		return false;
	}
	else
		return true;
}

function validateDestLoc()
{
	if (jQuery.trim(_destLocObj.value).length == 0)
	{
		focusDestinationLocation();
		manh.ils.form.handleError(document.getElementById("DestLocErrorMsg").value);
		return false;
	}
	else
		return true;
}	
