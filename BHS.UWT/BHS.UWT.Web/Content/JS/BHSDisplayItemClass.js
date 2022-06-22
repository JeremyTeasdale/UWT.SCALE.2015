$(function() {

    if (($('h3:contains(Putaway Confirmation)').length > 0)) {
        var item = document.getElementsByName("HIDDENOriginalItem")[0].value;
        var parms = "{'item':'" + item + "'}";
        $.ajax({
            type: "POST",
            url: "BHSDisplayItemClass.asmx/GetItemClass",
            data: parms,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(result) {
                alert(result.d);

                var newHTML = "<B>Stack Height:</B> " + result.d;

                var table = document.getElementsByTagName("table")[1];  // to get table
                var tbody = table.getElementsByTagName("tbody")[0]; // to get tbody

                var newRow = tbody.insertRow(tbody.rows.length - 1); // want to insert right above buttons
                var cell = newRow.insertCell(0);
                cell.innerHTML = newHTML;

            },
            error: function(xhr, err) {
                alert("error");
            }
        });
    }

});