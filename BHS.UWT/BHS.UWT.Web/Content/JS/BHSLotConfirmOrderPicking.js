function trim(str) {
    return str.replace(/^\s+|\s+$/g, '');
}

$(function() {

        // ILS built in function need to force to run to populate work instruction #
        prepareHistory();

        // parms
        var parms = {
            internalInstructionNum: $('[name=HIDDENinstrNumEdit]').val()
        };
                          
        $.ajax({
            type: "POST",
            url: "LotConfirmOrderPickingBHS.asmx/RequireLotValidation",
            data: JSON.stringify(parms),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(result) {
                                                
                if(result) {
                
                    $("td:contains('Lot:')").append('<input type="text" id="LotConfirmation" />');

                    // prevent keystroke
                    $('#LotConfirmation').keypress(function(e) {
                        e.preventDefault();
                    });
                }
            },
            error:function(xhr,err){ 
                alert("readyState: "+xhr.readyState+"\nstatus: "+xhr.status); 
                alert("responseText: "+xhr.responseText); 
            }
        });
        
        // if submit/ok is clicked, validate lot verfication
        $('[name=bOK]').click(function() {
        
            var lotConfirm = $('#LotConfirmation');
        
            if(lotConfirm.length)
            {
                var userInputLotNumber = lotConfirm.val();
                
                var actualLotNumber = $("td:contains('Lot:')").text();
                actualLotNumber = actualLotNumber.substring(4,actualLotNumber.length);
                                
                if(trim(userInputLotNumber.toUpperCase()) != trim(actualLotNumber.toUpperCase()))
                {
                    alert('Lot does not match lot on instruction.'); 
                    return false;
                }
            }
        });
    });

