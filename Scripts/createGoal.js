
//Google Tracking
(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments)
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
ga('create', 'UA-63033431-1', 'auto');
ga('send', 'pageview');

$(function start() {
    $("#datepicker").datepicker({
        onSelect: function (dateStr) {
            $("#dueDate").attr('value', dateStr);
           // alert($("#dueDate").value);
        }
    });
    
});


$(document).ready(function () {
    var counter = 2;
    var tour = createGoalPageTour();
//    var newuser = '<%=Session("newuser")%>';

    if (window.location.search.indexOf("gNum=") > -1)
        showModifyNotice();

    //if (newuser == "yes")
    //    startTutorial(tour);

    $('.tutorial').on('click', function () {
        startTutorial(tour);
    });

    $('#noticeButton').on('click', function () {
        $("#modifyNotice").toggle();
        $('#screen').toggle();
        $('#screen').css({});
        $('body').css({});

    })
});

function checkCreateGoalPageForErrors() {
    var hasError = true;

    try {
        if (goalTitle() == false) {
            if (goalDescription() == false)
                if (goalDate() == false)
                    if (goalFrequency() == false)
                        hasError = goalTimeText()
        }
    }
    catch (err) {
        hasError = true;
        $("#ErrorLabel").text("Javascript crash. Please take screen shot and send it in thanks.");
    }
    if (hasError == true)
        return false;
    else return true;
}

function checkError(hasError, field) {
    if (hasError == true) {
        field.style.borderWidth = "3px";
        field.style.borderColor = "red";
    }
    else {
        field.style.borderWidth = "";
        field.style.borderColor = "";
    }
}

function checkMultiTimeUnit() {
    var timetext = document.getElementById('timeText').value
    if (timetext < 2) {
        document.getElementById('timeUnitDropPlural').style.display = "none";
        document.getElementById('timeUnitDropSingle').style.display = "inline-block";
        document.getElementById('timeUnitSelect').value = "Single"
    }
    else {
        document.getElementById('timeUnitDropPlural').style.display = "inline-block";
        document.getElementById('timeUnitDropSingle').style.display = "none";
        document.getElementById('timeUnitSelect').value = "Plural"
    }
};

function checkMultiFreqUnit() {
    var timetext = document.getElementById('freqBox').value
    if (timetext < 2) {
        document.getElementById('freqDropDownPlural').style.display = "none";
        document.getElementById('freqDropDownSingle').style.display = "inline-block";
        document.getElementById('freqDropDownSelect').value = "Single"
    }
    else {
        document.getElementById('freqDropDownPlural').style.display = "inline-block";
        document.getElementById('freqDropDownSingle').style.display = "none";
        document.getElementById('freqDropDownSelect').value = "Plural"
    }
};

function checkNumberField(numField, fieldTitle) {
    var isNumbers = /^[1-9]+$/;
    var hasError = false;
    var numValue = 0;

    numValue = $(numField).val();
    if (numValue.length == 0) {
        $("#ErrorLabel").text("Activity " + fieldTitle + ". can't be blank");
        hasError = true;
    }
    else if (!numValue.match(isNumbers)) {
        $("#ErrorLabel").text("Bad Activity " + fieldTitle + ". Use Whole Numbers. Ex:3,10,5");
        hasError = true;
    };
    checkError(hasError, numField)
    return hasError;
}

function checkTextField(textField, textFieldName) {
    var onlyAllowLetters = /^[A-Za-z \s]+$/;
    var checkMultiSpaces = /^\s|\s$/;
    var fieldValue = textField.value.trim();
    var hasError = false;

    if (fieldValue.length < 3) {
        $("#ErrorLabel").text(textFieldName + " needs to be longer than 5 Characters.");
        hasError = true;
    }
    else if (fieldValue.match(checkMultiSpaces)) {
        $("#ErrorLabel").text(textFieldName + " field has multiple spaces together. ");
        hasError = true;
    }
    checkError(hasError, textField);
    return hasError;
}

function checkWord() {
    var freqnum = document.getElementById('freqBox').value
    if (freqnum < 2)
        document.getElementById('timeWord').innerText = "Time";
    else document.getElementById('timeWord').innerText = "Times";
};

function goalDate() {
    var goalDescValue = $("#datepicker").val();
    var today = new Date();
    var myDate = new Date(goalDescValue.substring(6, 10), goalDescValue.substring(0, 2) - 1, goalDescValue.substring(3, 5));
    var hasError = false;

    today.setHours(0, 0, 0, 0);
    if (myDate < today) {
        hasError = true;
        $("#ErrorLabel").text("Bad Goal Due Date. Need Valid Date of Today or Greater");
    }
    checkError(hasError, document.getElementById('datepicker-wrap'))
    return hasError;
}

function goalDescription() {
    return checkTextField(document.getElementById('goalDescText'), "Goal Description");
}

function goalFrequency() {
    return checkNumberField(document.getElementById('freqBox'), 'Frequency')
}


function goalTimeText() {
    return checkNumberField(document.getElementById('timeText'), 'Time')
}

function goalTitle() {
    return checkTextField(document.getElementById('goalTitleText'), "Goal Title");
}

function showModifyNotice() {
    $("#modifyNotice").toggle();
    $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
    $('body').css({ 'overflow': 'hidden' });
};