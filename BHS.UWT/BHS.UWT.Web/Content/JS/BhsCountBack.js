var CountBacklocation;
var CountBackItem;
var CountBackDesc;
var CountBackLot;
var CountBackQtyPicked;
var CountBack_CorrectQtyAtPageLoad;  //It is theoretically possible that someone could make a change to the location after the page loads but before the user completeds the count.  However this should be extreemly rare since he is physically at the location
var CountBack_Uoms;
var LastIIN;
var CorrectPassword;

var PreviousCount;

var Manh_body_OnKeyDown_Function;
var Manh_OnLoad;

$(function() {
    debug('BhsCountBack v0.15');
    RemoveSessionValues();
    if (window.prepareHistory) { //Not all of our pages contain this function.  
        prepareHistory();
    }

    Manh_OnLoad = window.window_OnLoad;
    window.window_OnLoad = (function() { });
    //The manh onload function shows shipment detail comments (if they exists) as an alert.  
    //countback logic needs to make 2 asynchronous, and depending on the outcome maybe show this message.
    //Since i don't know which one will finish first, i'll have them both flip this flag to false after running so that the one that finishes second does call the onload a second time.
    var ExecuteManhOnLoadFunction = true;

    //Hide the OH and ATP quantities from the picker so he cannot use them to cheat on the countback.
    var iin = GetCurrentInternalInstructionNum();
    if (iin != null && iin.length > 0 && $('h3:contains(Pick confirmation)').length > 0) {
        var parms = {
            internalInstructionNum: iin
        };

        $.ajax({
            type: "POST",
            url: 'BHSCountBack.asmx/CurrentWorkInstructionRequiresCountBack',
            data: JSON.stringify(parms),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(result) {
                debug('BHSCountBack.asmx/CurrentWorkInstructionRequiresCountBack: ' + result.d);
                if (result.d) {
                    AddSessionValues(iin);

                    var html = $('FORM').html();
                    debug(html);

                    var StartIndex = html.indexOf('OH', 0);
                    debug('start index: ' + StartIndex);
                    var EndIndex = html.indexOf('<TABLE', 0);
                    debug('end index: ' + EndIndex);

                    if (parseInt(StartIndex) >= 0 && parseInt(EndIndex) >= 0) {
                        var htmlPart1 = html.substring(0, StartIndex);
                        debug(htmlPart1);

                        var htmlPart2 = html.substring(EndIndex, 1000000);
                        debug(htmlPart2);

                        $('FORM').html(htmlPart1 + htmlPart2);
                    }
                }
                checkForCountBack();
            },
            error: function(xhr, err) {
                WebServiceError(xhr, err, "BHSCountBack.asmx/CurrentWorkInstructionRequiresCountBack", JSON.stringify(parms));
            }
        });
    } else {
        checkForCountBack();
    }

});

function checkForCountBack() {
    LastIIN = GetSessionValueLastInternalInstructionNum();

    if (LastIIN != null && LastIIN.length > 0) {
        var parms = {
            internalInstructionNum: LastIIN
        };

        $.ajax({
            type: "POST",
            url: 'BHSCountBack.asmx/LastWorkInstructionRequiresCountBack',
            data: JSON.stringify(parms),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(result) {
                debug('BHSCountBack.asmx/LastWorkInstructionRequiresCountBack: ' + result.d);
                if (result.d) {

                    //CorrectPassword = CallWebService("BHSCountBack.asmx/GetContinuePassword");

                    //Get Correct Continue Password
                    $.ajax({
                        type: "POST",
                        url: "BHSCountBack.asmx/GetContinuePassword",
                        //data: JSON.stringify(''),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function(result) {
                            CorrectPassword = result.d;
                            Debug_v2('CorrectPassword = ' + CorrectPassword);
                        },
                        error: function(xhr, err) {
                            WebServiceError(xhr, err, "BHSCountBack.asmx/GetContinuePassword", JSON.stringify(parms));
                        }
                    });
                    //End Get Correct Continue Password


                    debug('try to turn off manhattan functionality and enable counting back');
                    HideManhForm();
                    TakeEnterKeyFromManh();
                    ShowCountBackForm(LastIIN);
                }
                else {
                    debugShipComment('Call Manh Onload because the database says we do not require a count back');
                    Manh_OnLoad();
                }
            },
            error: function(xhr, err) {
				WebServiceError(xhr, err, "BHSCountBack.asmx/LastWorkInstructionRequiresCountBack", JSON.stringify(parms));
                Manh_OnLoad();
            }
        });
    } else {
        debugShipComment('Call Manh Onload because we did not find a previous IIN');
        Manh_OnLoad();
    }
}

function ShowCountBackForm(LastIIN) {
    $('body').append(BuildCountBackForm());

    var parms = {
        internalInstructionNum: LastIIN
    };

    $.ajax({
        type: "POST",
        url: 'BHSCountBack.asmx/GetWorkInstructionDetails',
        data: JSON.stringify(parms),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(result) {
            SetGlobalVarables(result.d);

            SetCountBackLocation();
            SetCountBackItem();
            SetCountBackDesc();
            SetCountBackLot();
			SetCountBackLpn(result.d.Lpn)

            $.ajax({
                type: "POST",
                url: 'BHSCountBack.asmx/RetrieveUOM',
                data: JSON.stringify(parms),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(result) {
                    CountBack_Uoms = result.d;
                    AddQtyFields();
                },
                error: function(xhr, err) {
					WebServiceError(xhr, err, "BHSCountBack.asmx/RetrieveUOM", JSON.stringify(parms));
                }
            });
        },
        error: function(xhr, err) {
			WebServiceError(xhr, err, "BHSCountBack.asmx/GetWorkInstructionDetails", JSON.stringify(parms));
        }
    });

}

function SetGlobalVarables(values) {
    CountBacklocation = values.location;
    CountBackItem = values.item;
    CountBackDesc = values.desc;
    CountBackLot = values.lot;
    CountBackQtyPicked = values.qtyPicked;
    CountBack_CorrectQtyAtPageLoad = values.correctQtyAtPageLoad;
}

function HideCountBackFrom() {
    $('#BhsCountBackFrom').hide();
}

function CountBackOK() {
    debug('CountBackOK()');
    SetMessage("");
    updateTotalQty();
    var CountedQty = GetCountedQty();

    if (isNaN(CountedQty) || parseInt(CountedQty) < 0) {
        SetMessage("Enter a non-negative whole number");
    } else {
        if (CountedQty == CountBack_CorrectQtyAtPageLoad && (PreviousCount == null || PreviousCount.length == 0)) {
            //The user counted matched expected on their first try.
            DoneWithCountBack(false);
        }
        else {
            //The user counted didn't match expected, or we are on a second try.
            if (PreviousCount == null || PreviousCount.length == 0) {
                //this the first count didn't match, double check picked qty
                SetMessage('Please Double check that ' + CountBackQtyPicked + ' was picked from this location and recount. (Comprueba que ' + CountBackQtyPicked + 'eran recolectados de esta ubicación y recuenta');
            }
            else {
                //we are on count 2 or greater
			
				if (CountedQty == CountBack_CorrectQtyAtPageLoad) {
					DoneWithCountBack(false);  //Our second count matched the expected quantity so we are good to go
				}
				else {
					invalidCountBackQty(CountedQty, LastIIN);  //Our second count didn't match so deal with the descrepancy
                }
				/*
                This logic was used to prompt for recounts until 2 matching counts where entered.  The Desired functionality has changed, and now we just want to submit the second count
                if (PreviousCount != CountedQty) {
                //our clast two counts did not match
                SetMessage('Your last two counts do not match.  Please recount.');
                }
                else {
                //our last two counts matched
                if (CountedQty == CountBack_CorrectQtyAtPageLoad) {
                DoneWithCountBack(false);
                } else {
                invalidCountBackQty(CountedQty, LastIIN);
                }
                }
                */
            }
        }
    }
    ZeroOutCountedQty();
    PreviousCount = CountedQty;
}

function invalidCountBackQty(CountedQty, InternalInstructionNum) {
    debug('invalidCountBackQty');
    var parms = {
        internalInstructionNum: InternalInstructionNum,
        countedQty: CountedQty
    };

    CallWebService('BHSCountBack.asmx/SubmitCountBackQty', parms);

    SetMessage('Unexpected Quantity Counted - wait for inventory control (Cantidad contada no cuadra: espera para Control de inventario)');

    debug('append continue button');

    var ContinueHtml =
                '<tr>' +
				'	<td> <input type="password" id="BhsContinuePass"> </td>' +
				'	<td> <input type ="button" id="BhsContinue" value = "Continue" onclick="DoneWithCountBack(true)"> </td>' +
				'</tr>';
	
	window.body_OnKeyDown = (function() {
        if (window.event.keyCode == 13) {
            DoneWithCountBack(true);
        }
        window.backButtonAccess();

    });

    $('#BhsCountOkButtonTd').append(ContinueHtml);
	$('BhsContinuePass').focus();
    debug('hide okay button');
    $('#BhsCountOkButton').hide()
}

//Helper Functions
function DoneWithCountBack(PasswordRequired) {
    Debug_v2('DoneWithCountBack('+PasswordRequired+')');
	
	if (PasswordRequired) {
        var UserPassword = $('#BhsContinuePass').val();
        Debug_v2('User Entered Password"' + UserPassword + '", and correct Password = "' + CorrectPassword + '"');

        if (UserPassword != CorrectPassword) {
            Debug_v2("Incorrect Pass");
            SetMessage("Incorrect Password (Contraseña incorrect)");
            return null;
        }
    }
	
	setTimeout(function () {
        HideCountBackFrom();
		ShowManhForm();
		ReturnEnterKeyToManh();
    }, 20);
}

function AddSessionValues(InternalInstructionNum) {
    debug('adding ' + InternalInstructionNum + ' to the session');
    var parms = {
        internalInstructionNum: InternalInstructionNum
    };

    CallWebService("BHSCountBack.asmx/AddSessionValues", parms);
}

//Remove Session Values
function RemoveSessionValues() {
    debug('RemoveSessionValues()');
    CallWebService("BHSCountBack.asmx/RemoveSessionValues");
}

function SetMessage(message) {
    debug('set message: ' + message);
    $('#BhsMessage').text(message);
}

function SetCountBackLocation() {
    $('#BhsCountBackLocation').text(CountBacklocation)
}

function SetCountBackItem() {
    $('#BhsCountBackItem').text(CountBackItem);
}

function SetCountBackDesc() {
    $('#BhsCountBackItemDesc').text(CountBackDesc);
}

function SetCountBackLot() {
    $('#BhsCountBackLot').text(CountBackLot);
}

function SetCountBackLpn(lpn) {
    $('#BhsCountBackLpn').text(lpn);
}

function AddQtyFields() {
    debug('AddQtyFields(' + CountBack_Uoms + ')');
    var UomHtml = '';
    $.each(CountBack_Uoms, function() {
        debug('Iterating over UOMs QuantityUM = ' + this.QuantityUM);
        UomHtml += '<tr> <td align="right">' + this.QuantityUM + ':</td> <td> <input type="text" onChange="return updateTotalQty()" id="Bhs' + this.QuantityUM + '" size="5" value="0" > </td> </tr>';
    });

    debug('adding UOM HTML' + UomHtml);

    if (UomHtml.length == 0) {
        $('#BhsCountBackQty').attr('readonly', false);
        $('#BhsCountBackQty').attr('style', "background:White");
    } else {
        $('#BhsAppendUomHere').append(UomHtml);
    }
    debug('done appending UOMS');
}

function updateTotalQty() {

    var Qty = 0;
    var QtyString;
    var error = false;

    $.each(CountBack_Uoms, function() {
        QtyString = $('#Bhs' + this.QuantityUM).val();
        if (!isNaN(QtyString) && parseInt(QtyString) >= 0) {
            //Qty += QtyString * (parseInt(jQuery.trim(this.ConversionQty.replace(',', ''))))
            Qty += parseInt(QtyString) * this.ConversionQty;
        }
        else {
            alert('Enter a non-negative whole number (Introduzca un número)');
            error = true;
        }
    });

    if (error) {
        $('#BhsCountBackQty').val('Error');
    }
    else {
        $('#BhsCountBackQty').val(Qty);
    }
}

function ZeroOutCountedQty() {
    debug('ZeroOutCountedQty()');
    $.each(CountBack_Uoms, function() {
        $('#Bhs' + this.QuantityUM).val('0');
    });

    updateTotalQty();
}

function BuildCountBackForm() {
    debug('BuildCountBackForm()');
    var html = '';
    html += '<table id="BhsCountBackFrom" align = "Center">';
    html += '	<tr  align="center" colSpan="2">';
    html += '		<td style="FONT-SIZE: 120%; FONT-FAMILY: Arial;">';
    html += '			<span>Count Back</span>';
    html += '		</td>';
    html += '	</tr>';
    html += '	<tr colSpan="2" align="Center">';
    html += '		<td>';
    html += '			<span id="BhsMessage" style="color:Red"></span>';
    html += '		</td>';
    html += '	</tr>';
    html += '	<tr colSpan="2"  align="Center">';
    html += '		<td>';
    html += '			<span>Count the quantity remaining in the location (Contar la cantidad restante en la ubicación)</span>';
    html += '		</td>';
    html += '	</tr>';
    html += '	<tr align="Center">';
    html += '		<td>';
    html += '			<table id="BHSDetailsTable">';
    html += '				<tr>';
    html += '					<td>Location:&nbsp&nbsp</td>';
    html += '					<td id="BhsCountBackLocation"></td>';
    html += '				</tr>';
    html += '				<tr>';
    html += '					<td>Item: </td>';
    html += '					<td id="BhsCountBackItem"></td>';
    html += '				</tr>';
    html += '				<tr>';
    html += '					<td></td>';
    html += '					<td id="BhsCountBackItemDesc"></td>';
    html += '				</tr>';
    html += '				<tr>';
    html += '					<td>Lot: </td>';
    html += '					<td id="BhsCountBackLot"></td>';
    html += '				</tr>';
    html += '				<tr id="BhsAppendUomHere">';
    html += '					<td>LPN: </td>';
    html += '					<td id="BhsCountBackLpn"></td>';
    html += '				</tr>';
    html += '				<tr id="BhsCountOkButtonTd">';
    html += '					<td align="right"> <input type ="button" id="BhsCountOkButton" value = "OK" onclick="CountBackOK()"> </td>';
    html += '					<td> <input type="text" id="BhsCountBackQty" size="5" value="0" readonly="true" style="background:lightgrey"> </td>';
    html += '				</tr>';
    html += '			</table>';
    html += '		</td>';
    html += '	</tr>';
    html += '</table>';

    return (html)
}

function HideManhForm() {
    debug('HideManhForm()');
    $('H3').hide();
    $('#FORM1').hide();
}

function ShowManhForm() {
    debug('ShowManhForm()');
    $('#FORM1').show();
    $('H3').show();
    debugShipComment('Call Manh Onload because we are re enabling the manh form');
    Manh_OnLoad();

}


function TakeEnterKeyFromManh() {
    debug('TakeEnterKeyFromManh()');

    Manh_body_OnKeyDown_Function = window.body_OnKeyDown;

    window.body_OnKeyDown = (function() {
        if (window.event.keyCode == 13) {
            CountBackOK();
        }
        window.backButtonAccess();

    });
}

function ReturnEnterKeyToManh() {
    debug('ReturnEnterKeyToManh()');
    //This method does not work properly unless we have already stored the Manh_body_OnKeyDown_Function
    if (Manh_body_OnKeyDown_Function != null) {
        window.body_OnKeyDown = Manh_body_OnKeyDown_Function;
    }
}

function GetCountedQty() {
    debug('GetCountedQty()');
    var qty = 'notFound';
    qty = $('#BhsCountBackQty').val()
    debug('found Quantity counted as: ' + qty);
    return qty;
}

//Get Last Internal Instruction Num
function GetSessionValueLastInternalInstructionNum() {
    debug('GetSessionValueLastInternalInstructionNum = ' + $('#LastInternalInstructionNum').val());
    return $('#LastInternalInstructionNum').val();
}

//Get Current Item
function GetItem() {
    return $('#itemNum').val();
}

//Get Current ISN
function GetCurrentInternalInstructionNum() {
    debug('found current IIN:' + $('[name=HIDDENinstrNumEdit]').val());
    return $('[name=HIDDENinstrNumEdit]').val();
}

//Pass a URL like "Bhs.PcmCapture.asmx/RemoveSessionValues", and optionally some parms
function CallWebService(Url, Parms) {
    debug('calling  WebService "' + Url + '" with parms ' + Parms);

    $.ajax({
        type: "POST",
        url: Url,
        data: JSON.stringify(Parms),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(result) {
            Debug_v2('(' + Url + ')result.d =' + result.d);
            return result.d;
        },
        error: function(xhr, err) {
			WebServiceError(xhr, err, Url, JSON.stringify(Parms));
        }
    });
}

function WebServiceError(xhr, err, WebService, ParmsString) {
	alert(
		"readyState: " + xhr.readyState +
		"\nstatus: " + xhr.status +
		"\n\nWeb Service '" + WebService + "' failed");
	alert("responseText: " + xhr.responseText);
	
	var parms = {
        WebService: WebService,
        ErrorMessage: xhr.responseText,
		OriginalParms: ParmsString
    };
	
	var url = "BHSCountBack.asmx/AuditLog"
	
	if(WebService != url) //We cannot log an error if its our error logging webservice that failed.  We'll just fail again and get caught in an infite loop of trying to log our failure to log
	{
		debug("Logging Audit");
		CallWebService(url, parms);
	}
}

function debug(message) {
    //comment out to disable debugging
    //alert(message);
}

function debugShipComment(message) {
    //comment out to disable debugging
    //'alert(message);
}

function Debug_v2(message) {
    //alert(message);
}