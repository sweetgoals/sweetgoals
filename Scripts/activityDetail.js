window.onload = function () {
    var select = document.getElementById('timeDropDown');
    var option = document.createElement('option');
    var checkChrome = Boolean(window.chrome);

    if ((select != null) & (option != null)) {
        option = document.createElement('option');
        option.text = option.value = "Manual";
        select.add(option, 0);
        for (var i = 3; i > -1 ; i--) {
            for (var j = 45; j > -1; j = j - 15) {
                option = document.createElement('option');
                if (j == 0)
                    option.text = option.value = i + ":00";
                else
                    option.text = option.value = i + ":" + j;
                if (!((i == 0) & (j == 0)))
                    select.add(option, 0);
            }
        }        
    }
    if (checkChrome==true)
        select.selectedIndex = 15;
    $("#timeDropDownValue").val("Manual");
};

$(document).ready(function () {
    var tour = detailActivityPageTour();
    var textControlServerData = $("#textControlServerData").val();
    var textControlSeverDataParsed = textControlServerData.split("<>");
    var textAreaReadSetting = 'textarea';
    var qString = location.search;

    function showPictureDialog(result) {
        $("#picShareSettingsSingle").val(result.d);
        $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
        $("#picShareSettingsSingle").focus();
    }

    $('#closeImg').on('click', function () {
        $("#addPicture").toggle();
        $('#screen').removeAttr("style");
    });

    $('#closePicSettings').on('click', function () {
        $("#picSettings").toggle();
        $('#screen').removeAttr("style");
    });

    $('#closeTitleDialog').on('click', function () {
        $("#titleDialog").toggle();
        $('#screen').removeAttr("style");
    });

    $('#closePicTimeDiv').on('click', function () {
        $("#manualTimeDiv").toggle();
        $('#screen').removeAttr("style");
    });

    $('#mainImg').on('click', function () {
        var loc = $('#mainImg').attr('src').replace("midsize", "actpic");
        window.open(loc, '_blank');
    });

    $('#manualTime').on('click', function () {
        $("#manualTimeDiv").toggle();
        $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
    });

    $('#menuPicture').on('click', function () {
        $("#addPicture").toggle();
        submitButtonClickSendDataServer();
        $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
        $("#addPicture").focus();
    });

    $('.picLine').on('click', function () {
        var mainImgFile = this.src.replace("thumbnail", "midsize");
        var newImg = new Image();
        var imgHeight = 480;
        var imgWidth = 620;

        $('#mainImg').attr('src', mainImgFile);
        newImg.src = mainImgFile;
        newImg.onload = function () {
            if (newImg.height < 480)
                imgHeight = newImg.height;
            if (newImg.width < 620)
                imgWidth = newImg.width;
            $('#mainImg').css({
                'height': imgHeight,
                'width': imgWidth,
                'margin-bottom': 480 - imgHeight,
                'margin-right': 620 - imgWidth});
        };
    });

    $('.sharePic').on('click', function () {
        var sharePicLoc = this.id;
        $("#picDisplayPanel").removeAttr("style");
        $("#picSettings").toggle();
        $("#pictureNumberHidden").val(sharePicLoc);
        $.ajax({
            type: "POST",
            url: "DetailActivity.aspx/getPicSettings",
            data: "{ 'id' : '" + sharePicLoc + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: showPictureDialog
        })
    });
    $('.file-upload').on('click', function () {
        $('#fileUploadTitle').focus();
    })

    $('#timeDropDown').change(function () {
        var val1 = $('#timeDropDown').val();
        if (val1 == 'Manual')
            $("#manualBox").toggle();
        else
            $("#manualBox").hide();
    })

    $('.tutorial').on('click', function () {
        startTutorial(tour);
    });

    if (($("#startActButton").is(':visible') == true) ||
        ($("#stopActButton").is(':visible') == true))
    {
        textAreaReadSetting = 'textarea';
        $('#mainSection').removeAttr("style");
        $('#activitySections').css('width', "91%");
        $('#controlSection').css('margin-bottom', '');
        $('#startSection').css('width', '100%');
        $('#startSection').css('border-bottom', '1px solid black');
    }
    else if (($("#editActButton").is(':visible') == true))
    {
        textAreaReadSetting = 'div';
        $("#sectionMenuColumn").toggle();
    }

    if (qString.indexOf("anum") == -1) {
        $('#mainSection').removeAttr("style");
        $('#activitySections').css('width', "91%");
        $('#statusDiv').css('display', 'none');
        $('#controlSection').css('margin-bottom', '');
        $('#startSection').css('border-bottom', '');
        $('#menuPicture').toggle();
        if (textControlSeverDataParsed[0].length==0)
            createTextControl();
    }

    if ($('.commentItem').length == 0)
        $('#actCommentPanel').removeClass('actCommentPanel');

    for (i = 0; i < textControlSeverDataParsed.length - 1; i += 3) {
        createTextControl(textControlSeverDataParsed[i], textControlSeverDataParsed[i + 1],
                          textControlSeverDataParsed[i + 2], textAreaReadSetting)
    }
    autosize(document.querySelectorAll('textarea'));
})


function changeTextControlTitle(textControlId) {
    $('#titleDialog').toggle();
    showOverlay();
    $('#titleControlId').val(textControlId);
    $('#titleControl').val($('#' + textControlId).text().trim())
    $('#titleControl').focus();
    $('#titleControl').val("")

}

function showFile() {
    var files = document.getElementById("picFile");
    var i = 0;

    $('#fileList').empty();
    for (i = 0; i < files.files.length; i++) {
        $('#fileList').append('<span class=fileNames>' + files.files[i].name + '</span>');
    }
}

function checkPics() {
    var numberFile = document.getElementById("picFile").files.length;
    var errorMsg = ""
    if (numberFile == 0) {
        errorMsg = "Need to have atleast one picture."
    }
    else if (numberFile > 20) {
        errorMsg = "Can only have twenty pictures added at once"
    }
    if (errorMsg.length > 0) {
        showErrorDialog(errorMsg);
        return false;
    }
    submitButtonClickSendDataServer();
};

function checkSessionValue(sessionValue) {
    if (sessionValue.length != 0)
        return true;
    else return false;
};

function closeErrorDialogButton() {
    if (($("#addPicture").is(':visible') == true) ||
        ($("#manualTimeDiv").is(':visible') == true) ||
        ($('#titleControl').is(':visible')==true)) {
        $('#errorDialog').toggle();
    }
    else {
        $('#errorDialog').toggle();
        $('#screen').toggle();
        $('#screen').css({});
        $('#errorDialog').css({});
        $('#screen').removeAttr("style");        
    }
    return false;
};

function createTextControl(titleData, listData, contentData, textAreaReadSetting) {
    if (titleData == undefined)
        createTextControlBlank();
    else
        createTextControlServerData(titleData, listData, contentData, textAreaReadSetting);
    autosize(document.querySelectorAll('textarea'));
}

function createTextControlBlank() {
    var i = 0;
    var controlCount = 0;
    var textControlTitleId = "";
    var textControlId = "";
    var textControlDeleteId = "";
    var textControlListId = "";
    var listType = "List";

    //make sure control ids aren't duplicated. 
    while ($("#textControl" + i).length > 0) {
        i++;
        if (i == 100)
            break;
    }
    controlCount = i;
    textControlTitleId = "textControltitle" + controlCount;
    textControlId = "textControl" + controlCount;
    textControlDeleteId = "deleteTextControlId" + controlCount;
    textControlListId = "listTextControlId" + controlCount;
    if (controlCount > 0)
        listType = "Activity";

    $('#sectionItemColumn').append(
        "<div id='textControl" + controlCount + "' class='block-textControl' >" +
        "   <div id='textControlHeaderDiv" + controlCount + "'>" +
        "       <span id='" + textControlTitleId + "' class='titleHeader' " +
        "           onclick=changeTextControlTitle('" + textControlTitleId + "')>Title </span>" +
        "       <span id='" + textControlDeleteId + "' class='deleteHeader' " +
        "           onclick=deleteTextControl('" + textControlId + "')>Delete</span>" +
        "       <span id='" + textControlListId + "' class='listHeader' " +
        "           onclick=textControlList('" + textControlListId + "')>" + listType + "</span>" +
        "   </div>" +
        "   <textarea id='activityTextControl_" + controlCount + "' class='block-info'" +
        "           placeholder='Text Control " + controlCount + "'></textarea>" +
        "</div>");
    $('#' + "activityTextControl" + controlCount).focus();
}

function createTextControlServerData(titleData, listData, contentData, textAreaReadSetting) {
    var controlCount = $('.block-textControl').length;
    var textControlTitleId = "textControltitle" + controlCount;
    var textControlId = "textControl" + controlCount;
    var textControlDeleteId = "deleteTextControlId" + controlCount;
    var textControlListId = "listTextControlId" + controlCount;
    var titleOnClick = "changeTextControlTitle('" + textControlTitleId + "')";
    var deleteOnClick = "deleteTextControl('" + textControlId + "')";
    var listOnClick = "textControlList('" + textControlListId + "')";
    var headerControlsClass = 'headerControlsClassOn';
    var titleHeaderClass = 'titleHeader';
    var textAreaClass = 'block-info';
    var contentDataControl = "";

    if (textAreaReadSetting.indexOf('div') >= 0) {
        titleOnClick = "";
        deleteOnClick = "";
        listOnClick = "";
        titleHeaderClass = '';
        headerControlsClass = 'headerControlsClassOff';
        textAreaClass = 'block-info-off';
    };

    contentDataControl = "<" + textAreaReadSetting + " id='activityTextControl" + controlCount + "' class='" +
                            textAreaClass + "' placeholder='Text Control " + controlCount + "'></" +
                            textAreaReadSetting + ">"

    if (contentData.length != 0) {
        contentDataControl = "<" + textAreaReadSetting + " id='activityTextControl" + controlCount + "' class='" +
                                textAreaClass + "' placeholder='Text Control " + controlCount + "'>" + contentData +
                                "</" + textAreaReadSetting + ">"
    }

    $('#sectionItemColumn').append(
        "<div id='textControl" + controlCount + "' class='block-textControl' >" +
        "   <div id='textControlHeaderDiv" + controlCount + "'>" +
        "       <span id='" + textControlTitleId + "' class='" + titleHeaderClass + "' " +
        "           onclick=" + titleOnClick + ">" + titleData + " </span>" +
        "       <div class='" + headerControlsClass + "'>" +
        "           <span id='" + textControlDeleteId + "' class='deleteHeader' " +
        "                 onclick=" + deleteOnClick + ">Delete</span>" +
        "           <span id='" + textControlListId + "' class='listHeader' " +
        "                 onclick=" + listOnClick + ">" + listData + "</span>" +
        "       </div>" +
        "   </div>" +
        contentDataControl +
        "</div>");
    $('#' + "activityTextControl" + controlCount).focus();
}

function deleteTextControl(textControlId) {
    var controlCount = $('.block-textControl').length;
    if (controlCount != 1)
        $("#" + textControlId).remove();
}

function elementCheckLengthHide(checkElement, hideElement) {
    if ($(checkElement).text().length == 0)
        hideElement.toggle();
};

function loadItemHiddenField(itemHiddenField, itemField, blockName) {
    if ($(itemField).val().length > 0) {
        if (blockName.css('display') == 'block') {
            itemHiddenField.val(itemField.val());
            return true;
        }
    }
    else return false;
}

function renameTextControlEnter(e) {
    if (e.keyCode == 13) {
        renameTextControl();
        return false;
    }    
}

function renameTextControl() {
    var textControlId = $('#titleControlId').val();
    var valid = false;

    if (($('#titleControl').val().length > 0) && ($('#titleControl').val().length < 51)){
        valid=true;
        $('#' + textControlId)[0].innerText = $('#titleControl').val();
    }
    else if ($('#titleControl').val().length > 49) {
        showErrorDialog("Title has to be less than 50 characters");
    }
    else {
        valid = true;
        $('#' + textControlId)[0].innerText = "Title";
    }
    if(valid==true){
        $('#titleControl').val('');
        $('#titleDialog').toggle();
        $('#screen').removeAttr("style");
    }
    return false;
};

function savePictureSettings() {
    var pn = $("#pictureNumberHidden").val();
    var pns = $("#picShareSettingsSingle").val();
    var sendData = $("#pictureNumberHidden").val() + "," + $("#picShareSettingsSingle").val();
    var blah = "{ 'picNumber' : '" + $("#pictureNumberHidden").val() + "', 'picNewSetting : '" +
                    $("#picShareSettingsSingle").val() + "' }"
    $.ajax({
        type: "POST",
        url: "DetailActivity.aspx/savePicturesettings",
        data: "{ 'sendData' : '" + sendData + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: savePictureSettingsSuccess
    })
}

function savePictureSettingsSuccess(result) {
    $("#picSettings").toggle();
    $('#screen').removeAttr("style");
}

function setHidden(value) {
    document.getElementById('timeDropDownValue').value = value;
}

function showControl(divId, blockId) {
    var bId = $(blockId)[0].id;

    $(divId).toggle();
    if ($(blockId)[0].innerText.length == 0) {
        $(blockId).remove();
        $(divId).append("<textarea id=" + bId + " class='block-info'></textarea>");
        $('#' + bId).focus();
    }
    return false;
}

function showErrorDialog(errorMsg) {
    // to use this you also need to set the dialog name in closeErrorDialogButton function. Right now
    // #addPicture, #manualTimeDiv, #titleControl are using this. 
    $('#errorDialogText').text(errorMsg);
    $('#errorDialog').css({ 'display': 'block', 'z-index': '1000' });
    $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
};

function showOverlay() {
    $('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
}

function submitButtonClick() {
    var returnValue = true;

    returnValue = submitButtonClickCheckListSetting();   
    if (returnValue==true)
        returnValue = submitButtonClickSendDataServer();

    return returnValue;
}

function submitButtonClickDialog() {
    var returnValue = true;

    if ($("#manualBox").is(':visible') == true) {
        var checkManualBox = $("#manualBox").val();
        if (checkManualBox.length == 0) {
            showErrorDialog("Can't have a blank time. Format hh:mm. Ex: 0:39, 1:32");
            return false;
        }
        else if (checkManualBox.indexOf(":") > 0) {
            returnValue = submitButtonClickCheckManual();
            if (returnValue == false)
                return false;
        }
        else {
            showErrorDialog("Bad Time. Format hh:mm. Ex: 0:39, 1:32");
            return false;
        }
    }
    else
        $("#manualBox").val($("#timeDropDownValue").val());

    submitButtonClick();
}

function submitButtonClickCheckListSetting() {
    var i = 0;
    var checkTextControlLists = $('.listHeader');
    var checkErrorLabelList = $('.errorLabel');
    var listedControlCount = 0;

    for (i = 0; i < checkTextControlLists.length; i++) {
        if (checkTextControlLists[i].innerHTML.indexOf("List") > -1) {
            listedControlCount += 1;
            if (listedControlCount > 1){
                checkTextControlLists[i].className = "errorLabel";
            }
        }
    }
    for (i = 0; i < checkErrorLabelList.length; i++) {
        if (checkErrorLabelList[i].innerHTML.indexOf("List") > -1) {
            listedControlCount += 1;
        }
    }
    if (listedControlCount == 0 && ($('.errorLabel').length == 0)) {
        checkTextControlLists[0].className = "errorLabel";
    }

    if (listedControlCount != 1) {
        showErrorDialog("Must have one Text Control set to 'List'. All other Text Controls need " +
                        "to be set to 'Activity'. Be sure to change the control in red before continuing. ");
        return false;
    }
    else if ($('.errorLabel').length > 0) {
        submitButtonClickCheckListSettingClearError();
    }
    return true;
}

function submitButtonClickCheckListSettingClearError() {
    var errorLabels = $('.errorLabel');
    for (i = 0; i < errorLabels.length; i++) {
        errorLabels[i].className = "listHeader";
        errorLabels[i].innerText = "Activity";
    }
    //$('#screen').css({ opacity: 0.7, 'width': $(document).width(), 'height': $(document).height() });
    //$('body').css({ 'overflow': 'hidden' });
    //$('#errorDialog').css({ 'display': 'block' });
};

function submitButtonClickCheckManual() {
    var splitBox = $('#manualBox').val().split(":");
    var sHour = parseFloat(splitBox[0]);
    var sMinute = parseFloat(splitBox[1]);
    var boxTime = new Date(0, 0, 0, splitBox[0], splitBox[1], 0, 0);
    var hours = boxTime.getHours();
    var minutes = boxTime.getMinutes();
    var error = false;

    if ((sHour > 23) || (sHour < 0))
        error = true;
    else if ((sMinute < 0) || (sMinute > 59))
        error = true;
    else if (isNaN(hours))
        error = true;
    else if (isNaN(minutes))
        error = true;
    else if ((sHour == 0) && (sMinute == 0))
        error = true;
    if (error == true) {
        showErrorDialog("Bad Time. Format hh:mm. Ex: 0:39, 1:32");
        return false;
    }
    return true;
}

function submitButtonClickSendDataServer() {
    var i = 0;
    var textControls = $('.block-textControl');
    var textControlData = "";
    var textControlTitle = "";
    var textControlContent = "";
    var textControlList = "";

    try{
        for (i = 0; i < textControls.length; i++) {
            textControlTitle = $("span.titleHeader")[i].innerHTML;
            textControlList = $("span.listHeader")[i].innerHTML;
            textControlContent = $(".block-info")[i].value;

            textControlData += textControlTitle + "[break]";
            textControlData += textControlList + "[break]";
            textControlData += textControlContent + "[control]";
        }
        $("#textControlDataHidden").val(textControlData);
        return true;
    }
    catch (err) {
        alert(err);
        return false;
    }
}

function textControlList(listControlId) {
    $("#" + listControlId)[0].className = "listHeader";
    if ($("#" + listControlId)[0].innerHTML == "List")
        $("#" + listControlId).text("Activity");
    else
        $("#" + listControlId).text("List");    
}