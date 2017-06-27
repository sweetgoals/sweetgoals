
$(document).ready(function () {
        $('.linky').on('click', function () {
            alert(this.text());
            $("#deleteAccount").dialog('open');
            return false;
        });

        $("#addSupporter", "#addGoal", "#deleteAccount", "#deleteGoal").dialog({
            autoOpen: false,
            width: 300,
            height: 275,
        });

        $("#deleteAccount").dialog({
            autoOpen: false,
            width: 400,
            height: 275,
            buttons: [
              {
                  text: "OK",
                  click: function () {
                          
                      $.ajax({
                          type: "POST",
                          url: "dailog.aspx/removeAccountSupport",
                          data: JSON.stringify({
                              "dasupemail": $("#dasupemail").val(),
                              "dasupmsg": $("#dasupmsg").val(),
                              "gn": '<%=Session("gN")%>',
                          }),
                          contentType: "application/json; charset=utf-8",
                          dataType: "json",
                          success: onSuccess,
                          error: errormsg,
                      })
                      $("#dasupemail").val("");
                      $("#dasupmsg").val("");
                      $(this).dialog("close");
                  }
              }
            ]
        });

    });
