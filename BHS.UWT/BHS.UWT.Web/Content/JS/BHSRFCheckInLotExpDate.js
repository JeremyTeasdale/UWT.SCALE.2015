$(function() {

        // parms
        var parms = {
            internalReceiptNum: $('[name=HIDDENINTRECNUM]').val(),
            lot: $('[name=HIDDENLOT]').val()
        };
        
        alert(parms.internalReceiptNum);
        alert(parms.lot);
                          
        $.ajax({
            type: "POST",
            url: "RFCheckInLotExpDateBHS.asmx/GetPreviousLotExpDate",
            data: JSON.stringify(parms),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(result) {
                alert(result);
            },
            error:function(xhr,err){ 
                alert("readyState: "+xhr.readyState+"\nstatus: "+xhr.status); 
                alert("responseText: "+xhr.responseText); 
            }
        });
    });

