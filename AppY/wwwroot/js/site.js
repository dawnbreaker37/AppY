let fullWidth = 0;
let fullHeight = 0;
let botOffNavbarH = 0;
let alertTimeout;
let letters = /^[A-Za-z]+$/;

window.onload = function () {
    fullHeight = parseInt(window.innerHeight);
    fullWidth = parseInt(window.innerWidth);
    botOffNavbarH = parseInt($("#MainBotOffNavbar").innerHeight());
    displayCorrect(fullWidth);
    additionalBtnSelector(fullWidth);

    if (fullWidth >= 768) {
        animatedOpen(false, "SmallsidePreloaded_Container", true, false);
        if ($("#SmallsidePreloaded_Container").css("display") == "block") {
            setTimeout(function () {

                animatedOpen(false, "Preloaded_Container", true, false);
            }, 650);
        }
        else {
            animatedOpen(false, "Preloaded_Container", true, false);
        }
    }
    else {
        animatedOpen(false, "Preloaded_Container", true, false);
    }
}
window.onresize = function () {
    fullHeight = parseInt(window.innerHeight);
    fullWidth = parseInt(window.innerWidth);
    botOffNavbarH = parseInt($("#MainBotOffNavbar").innerHeight());
    displayCorrect(fullWidth);
    additionalBtnSelector(fullWidth);
}


$("#SignUp_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();
    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-circle-check text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 5);
            $("#ReserveCode_Lbl").text(response.reserveCode);
            animatedOpen(false, "SigningUpFinished_Container", true, true);
        }
        else {
            if (response.error != null) {
                alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert + response.error, "Got It", null, 0, null, null, null, 4.65);
            }
            else alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.65);
        }
    });
});
$("#LogIn_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();
    $.post(url, data, function (response) {
        if (response.success) {
            document.location.href = "/Home/Index";
        }
        else {
            $("#LogIn_Password_Val").val(null);
            alert('<i class="fa-solid fa-user-xmark text-neon-purple"></i>', response.alert, "Close", null, 0, null, null, null, 3.75);
        }
    });
});

$("#SendCodeAgain_Btn").on("click", function () {
    $("#SendSingleUseCode_Form").submit();
    $(this).attr("disabled", true);
    $(this).text("Resend Code");
});
$("#SendSingleUseCode_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let timerDuration = 100;
            let resendInterval = setInterval(function () {
                timerDuration--;
                $("#SendCodeAgain_Btn").attr("disabled", true);
                $("#SendCodeAgain_Btn").text("Resend from " + timerDuration + " sec");
            }, 1000);
            setTimeout(function () {
                $("#SendCodeAgain_Btn").text("Resend Code Now");
                $("#SendCodeAgain_Btn").attr("disabled", false);
                clearInterval(resendInterval);
            }, timerDuration * 1000);

            animatedClose(false, "ForgotPassword_Container", true, true);
            animatedOpen(false, "SubmitCode_Container", true, false);
            $("#SubmitSingleUsingCode_Email_Val").val(response.email);
            alert('<i class="fa-regular fa-paper-plane text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 5);
        }
        else {
            $("#SingleUseCode_Email_Val").val(null);
            alert('<i class="fa-regular fa-rectangle-xmark text-danger"></i>', response.alert, "Okay", null, 0, null, null, null, 5);
        }
    });
});
$("#SubmitSingleUsingCode_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-circle-check text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 4.75);
            $("#UpdatePassword_Token_Val").val(response.token);
            $("#UpdatePassword_Email_Val").val(response.email);
            animatedClose(false, "SubmitCode_Container", true, true);
            animatedOpen(false, "CreateNewPassword_Container", true, false);
        }
        else {
            $("#SubmitSingleUsingCode_Code_Val").val(null);
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#SubmitAccountViaReserveCode_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-circle-check text-neon-purple"></i>', response.alert, "Close", null, 0, null, null, null, 4.25);
            $("#UpdatePassword_Token_Val").val(response.token);
            $("#UpdatePassword_Email_Val").val(response.email);
            animatedClose(false, "ForgotPassword_Container", true, true);
            animatedOpen(false, "CreateNewPassword_Container", true, false);
        }
        else {
            $("#SendReserveCodeViaEmail_ReserveCode_Val").val(null);
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

$("#UpdatePassword_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let currentUrl = document.location.href;
            if (currentUrl.toLowerCase().includes("/user/account")) {
                alert('<i class="fa-solid fa-check-double text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
                $("#UpdatePassword_Password_Val").attr("disabled", true);
                $("#UpdatePassword_ConfirmPassword_Val").attr("disabled", true);
                $("#Password").attr("disabled", true);
                $("#ConfirmPassword").attr("disabled", true);
                $("#UpdatePassword_SbmtBtn").attr("disabled", true);
                $("#UpdatePassword_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> Password Updated');

                $("#ChangePassword_Box").fadeOut(300);
                $("#SubmitPassword_Box").fadeOut(300);
                $("#PasswordChangeData_Box").fadeIn(300);
                $("#DayOfChange_Lbl").text("few moments ago");
                $("#DaysPassed_Lbl").text("today, few minutes ago");
            }
            else {
                alert('<i class="fa-regular fa-circle-check text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 3.75);
                $("#UpdatePassword_Password_Val").attr("disabled", true);
                $("#UpdatePassword_ConfirmPassword_Val").attr("disabled", true);
                $("#UpdatePassword_SbmtBtn").attr("disabled", true);
                $("#UpdatePassword_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> Password Updated');
            }
        }
        else {
            $("#UpdatePassword_Password_Val").val(null);
            $("#UpdatePassword_ConfirmPassword_Val").val(null);
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Try Again", null, 0, null, null, null, 4.5);
        }
    });
});

$("#ChangePassword_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-solid fa-check-double text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
            animatedClose(false, "SecuritySettings_Container", true, true);
            setTimeout(function () {
                $("#ChangePassword_Box").fadeOut(100);
                $("#SubmitPassword_Box").fadeOut(100);
                $("#PasswordChangeData_Box").fadeIn(100);
                $("#DayOfChange_Lbl").text("few moments ago");
                $("#DaysPassed_Lbl").text("today, few minutes ago");
            }, 500);
            setTimeout(function () {
                animatedOpen(false, "SecuritySettings_Container", true, false);
            }, 600);
        }
        else {
            $("#OldPassword").val(null);
            $("#ChangePassword_NewPassword").val(null);
            $("#ChangePassword_ConfirmPassword").val(null);
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});

$("#EditAccount_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-pen-to-square text-neon-purple"></i>', response.alert, "Done", null, 0, null, null, null, 4);
            if (response.result.shortName != $("#InitialShortname_Val").val()) {
                $("#ShortnameVal_Span").text(response.result.shortName.toLowerCase());
                $("#AvatarDesignShortname_Lbl").text("@" + response.result.shortName.toLowerCase());
                $("#InitialShortname_Val").val(response.result.shortName.toLowerCase());
                $("#UserInfo_Shortname_Lbl").text("@" + response.result.shortName.toLowerCase());
            }

            $("#UnpicturedAvatar_Lbl").text(response.result.pseudoName[0]);
            $("#UserInfo_Pseudoname_Lbl").text(response.result.pseudoName);
            $("#AvatarDesign_Container_MainLbl").text(response.result.pseudoName);
            if (response.result.description != null) {
                $("#UserInfo_Description_Lbl").html(response.result.description);
            }
            else {
                $("#UserInfo_Description_Lbl").html("No description");
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

$("#EditAvatarDesign_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            if (fullWidth < 768) {
                closeSidebar();
                animatedClose(false, "AvatarDesign_Container", true, true);
            }
            $("#UnpicturedAvatar_Lbl").css("color", "#" + response.fgColor);
            $("#UnpicturedAvatarSm_Lbl").css("color", "#" + response.fgColor);
            $("#AvatarStyleChange_MainLbl").css("color", "#" + response.fgColor);
            $("#AvatarNone_Box").css("background-color", "#" + response.bgColor);
            $("#AvatarNoneSm_Box").css("background-color", "#" + response.bgColor);
            $("#AvatarStyleChange_Box").css("background-color", "#" + response.bgColor);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});

$("#IsEmailUnique_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();
    $.get(url, data, function (response) {
        if (!response.success) {
            $("#EmailStatus_Lbl").html(' <i class="fa-regular fa-circle-check text-primary"></i> Email is vacant');
            $("#EmailStatus_Lbl").removeClass("text-danger");
            $("#EmailStatus_Lbl").addClass("text-muted");
            $("#SignUp_Box-NextSlide").attr("disabled", false);
        }
        else {
            $("#SignUp_Form-SbmtBtn").attr("disabled", true);
            $("#SignUp_Box-NextSlide").attr("disabled", true);
            $("#EmailStatus_Lbl").addClass("text-danger");
            $("#EmailStatus_Lbl").removeClass("text-muted");
            $("#EmailStatus_Lbl").html(' <i class="fa-regular fa-circle-xmark text-danger"></i> This email address is busy');
        }
    });
});
$("#IsUsernameUnique_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();
    $.get(url, data, function (response) {
        if (!response.success) {
            $("#UsernameStatus_Lbl").html(' <i class="fa-regular fa-circle-check text-primary"></i> Username is vacant');
            $("#UsernameStatus_Lbl").removeClass("text-danger");
            $("#UsernameStatus_Lbl").addClass("text-muted");
            $("#SignUp_Box-NextSlide").attr("disabled", false);
        }
        else {
            $("#SignUp_Form-SbmtBtn").attr("disabled", true);
            $("#SignUp_Box-NextSlide").attr("disabled", true);
            $("#UsernameStatus_Lbl").addClass("text-danger");
            $("#UsernameStatus_Lbl").removeClass("text-muted");
            $("#UsernameStatus_Lbl").html(' <i class="fa-regular fa-circle-xmark text-danger"></i> This username is busy');
        }
    });
});
$("#IsShortnameUnique_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#ShortnameVal_Span").text(response.result);
            $("#ShortnameStatus_Span").html("<span class='text-primary'>This shortname is free to use</span>");
            $("#EditAccount_SbmtBtn").attr("disabled", false);
            $("#EditAccount_SbmtBtn").text("Save");
        }
        else {
            $("#ShortnameVal_Span").text($("#InitialShortname_Val").val());
            $("#ShortnameStatus_Span").html("<span class='text-danger'>This shortname has already been taken</span>");
            $("#EditAccount_SbmtBtn").attr("disabled", true);
            $("#EditAccount_SbmtBtn").text("Edit Shortname to Save");
        }
    });
});
$("#Email").on("change", function () {
    $("#IsEmailUnique_Email_Val").val($(this).val());
    $("#IsEmailUnique_Form").submit();
});
$("#Username").on("change", function () {
    $("#IsUsernameUnique_Email_Val").val($(this).val());
    $("#IsUsernameUnique_Form").submit();
});
$("#ShortName").on("change", function () {
    if ($(this).val().length >= 4) {
        $("#IsShortnameUnique_Shortname_Val").val($(this).val());
        $("#IsShortnameUnique_Form").submit();
    }
});
$("#ConfirmPassword").on("keyup", function () {
    let thisVal = $(this).val();
    let passwordVal = $("#Password").val();
    if (thisVal === passwordVal) {
        $("#SignUp_Form-SbmtBtn").attr("disabled", false);
    }
    else {
        $("#SignUp_Form-SbmtBtn").attr("disabled", true);
    }
});

$("#LogIn_Email_Val").on("change", function () {
    let val = $(this).val();
    if (val.includes("@") && val.includes(".")) {
        $("#IsViaUsername_Val").val(false);
    }
    else {
        $("#IsViaUsername_Val").val(true);
    }
});

$("#SubmitSingleUsingCode_Code_Val").on("keyup", function () {
    let value = $(this).val();
    if (value.length >= 8) {
        $("#SubmitSingleUsingCode_Form").submit();
    }
});

$("#ChangeToReserveCodeType_Btn").on("click", function () {
    animatedClose(false, "ForgotPassword_Container", false, true);
    setTimeout(function () {
        $("#RecoverViaEmail_Box").fadeOut(0);
        $("#RecoverViaReserveCode_Box").fadeIn(0);
        animatedOpen(false, "ForgotPassword_Container", true, false);
    }, 450);
});
$("#ChangeToSingleUseType_Btn").on("click", function () {
    animatedClose(false, "ForgotPassword_Container", false, true);
    setTimeout(function () {
        $("#RecoverViaEmail_Box").fadeIn(0);
        $("#RecoverViaReserveCode_Box").fadeOut(0);
        animatedOpen(false, "ForgotPassword_Container", true, false);
    }, 450);
});

$(document).on("keyup change", ".form-control-check-input", function () {
    let neededBtn = $(this).attr("data-bs-html");
    let minLength = parseInt($(this).attr("data-bs-min-length"));
    let currentLength = parseInt($(this).val().length);

    if (currentLength >= minLength) $("#" + neededBtn).attr("disabled", false);
    else $("#" + neededBtn).attr("disabled", true);
});

$(document).on("click", ".btn-open-container", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        animatedOpen(false, trueId, true, true);
    }
});
$(document).on("click", ".btn-smallside-open-container", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        animatedClose(true, "smallside-box-container", true, true);
        if (fullWidth < 768) {
            $("#Main_SideBar").fadeIn(0);
            $("#Main_SideBar").css("left", 0);
            setTimeout(function () {
                animatedOpen(false, trueId, true, false);
            }, 190);
        }
        else {
            setTimeout(function () {
                animatedOpen(false, trueId, true, false);
            }, 190);
        }
    }
});

$(document).on("click", ".btn-slide-to-next", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let currentVal = parseInt($("#" + trueId + "-StepVal").val());
        let maxVal = parseInt($("#" + trueId + "-MaxStepVal").val());
        if (currentVal < maxVal) {
            currentVal++;
            $("#" + trueId + "-StepVal").val(currentVal);
            slide(false, trueId + "-Part" + currentVal);

            if (currentVal > 0) {
                $("#" + trueId + "-PrevSlide").attr("disabled", false);
            }
            if (currentVal >= maxVal) $("#" + event.target.id).attr("disabled", true);
            else $("#" + event.target.id).attr("disabled", false);
        }
    }
});
$(document).on("click", ".btn-slide-to-prev", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let currentVal = parseInt($("#" + trueId + "-StepVal").val());
        let maxVal = parseInt($("#" + trueId + "-MaxStepVal").val());
        if (currentVal > 0) {
            currentVal--;
            $("#" + trueId + "-StepVal").val(currentVal);
            slide(false, trueId + "-Part" + currentVal);
            if (currentVal < maxVal) {
                $("#" + trueId + "-NextSlide").attr("disabled", false);
            }
            else $("#" + trueId + "-NextSlide").attr("disabled", true);

            if (currentVal <= 0) $("#" + event.target.id).attr("disabled", true);
            else $("#" + event.target.id).attr("disabled", false);
        }
        else {
            $("#" + event.target.id).attr("disabled", true);
        }
    }
});

$(document).on("click", ".btn-close-sidebar", function () {
    closeSidebar();
});
$(document).on("click", ".btn-close", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        if ($("#" + trueId).hasClass("smallside-box-container")) {
            animatedClose(false, trueId, true, true);
        }
        else {
            let typeSelector = Math.floor(Math.random() * 2);
            if (typeSelector === 0) animatedClose(false, trueId, true, false);
            else animatedClose(false, trueId, false, false);  
        } 
    }
});

$(document).on("click", ".open-sidebar", function () {
    openSidebar();
});

$(document).on("keydown", ".strong-input", function (event) {
    let isShiftKeyed = event.shiftKey;

    if (event.which >= 48 && event.which <= 57 && isShiftKeyed) return false;
    else if (event.which >= 48 && event.which <= 57 && !isShiftKeyed) return true;
    else if (event.which == 8) return true;
    else if (event.which >= 65 && event.which <= 90) {
        if (event.key.match(letters)) {
            return true;
        }
        else return false;
    }
    else if (event.which == 189 && isShiftKeyed) return true;
    else if (event.which >= 97 && event.which <= 122) return true;
    else return false;
});

$(document).on("click", ".copy-to-clipboard", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        navigator.clipboard.writeText($("#" + trueId).html());
        alert('<i class="fa-regular fa-copy text-neon-purple"></i>', "Text has been successfully copied to clipboard", "Done", null, 0, null, null, null, 3.5);
    }
});

$(document).on("click", ".set-from-into", function (event) {
    let setInto = getTrueId(event.target.id);
    if (setInto != null) {
        let setFrom = $("#" + event.target.id).attr("data-bs-set-from");
        $("#" + setInto).val($("#" + setFrom).text());
        $("#" + setInto).change();
        alert('<i class="fa-solid fa-circle-right text-success"></i>', "Text has been successfully placed at his place", "Done", null, 0, null, null, null, 3.5);
    }
});

$(document).on("click", ".set-style", function (event) {
    let trueId = parseInt(getTrueId(event.target.id));
    if (trueId != null) {
        let currentUrl = document.location.href;
        $("#TextType_Val").val(trueId);

        switch (trueId) {
            case 4:
                $("#TES_Type_Span").text("Colorful Text");
                $("#PreparedText1_Lbl").text("Color Code (#HEX)");
                $("#PreparedText1_Val").attr("placeholder", "#0d6efd");
                $("#PreparedText2_Lbl").text("Input Text");
                $("#PreparedText2_Val").attr("placeholder", "Enter your text here");
                break;
            case 5:
                $("#TES_Type_Span").text("Text with Link");
                $("#PreparedText1_Lbl").text("Link");
                $("#PreparedText1_Val").attr("placeholder", "Put the link here");
                $("#PreparedText2_Lbl").text("Main Text");
                $("#PreparedText2_Val").attr("placeholder", "Enter your text here");
                break;
            case 6:
                $("#TES_Type_Span").text("Tooltip");
                $("#PreparedText1_Lbl").text("Tooltip Text");
                $("#PreparedText1_Val").attr("placeholder", "Enter tooltip text");
                $("#PreparedText2_Lbl").text("Main Text");
                $("#PreparedText2_Val").attr("placeholder", "Enter your text here");
                break;
            default:
                $("#TES_Type_Span").text("Colorful Text");
                $("#PreparedText1_Lbl").text("Color Code (#HEX)");
                $("#PreparedText1_Val").attr("placeholder", "#0d6efd");
                $("#PreparedText2_Lbl").text("Input Text");
                $("#PreparedText2_Val").attr("placeholder", "Enter your text here");
                break;
        }

        if (currentUrl.toLowerCase().includes("/toolbox")) animatedOpen(false, "TextEditorSettings_Container", true, true);
        else animatedOpen(false, "TextEditorSettings_Container", true, true);
    }
});

$("#SetDefaultColors_Btn").on("click", function () {
    $("#AvatarBgColor_Val").val("f8f9fa");
    $("#AvatarFgColor_Val").val("000000");
    $("#AvatarBgColor_Val").change();
    $("#AvatarFgColor_Val").change();
});
$("#SetRandomColors_Btn").on("click", function () {
    let red = 0;
    let green = 0;
    let blue = 0;
    let bgColor = "f8f9fa";
    let fgColor = "0d6efd";

    for (let i = 0; i < 2; i++) {
        red = Math.floor(Math.random() * 255);
        green = Math.floor(Math.random() * 255);
        blue = Math.floor(Math.random() * 255);

        if (i == 0) bgColor = rgbToHex(red, green, blue);
        else fgColor = rgbToHex(red, green, blue);
    }

    if (bgColor != fgColor) {
        $("#AvatarBgColor_Val").val(bgColor);
        $("#AvatarFgColor_Val").val(fgColor);
        $("#AvatarBgColor_Val").change();
        $("#AvatarFgColor_Val").change();
    }
    else $(this).click();
});

$("#AvatarBgColor_Val").on("change", function () {
    let value = $(this).val();
    if (value.length == 6) {
        $("#AvatarDesignExample-Box").css("background-color", "#" + value);
    }
    else $("#AvatarDesignExample-Box").css("background-color", "#f0f0f0");
});
$("#AvatarFgColor_Val").on("change", function () {
    let value = $(this).val();
    if (value.length == 6) {
        $("#AvatarDesign_Container_Lbl").css("color", "#" + value);
        $("#AvatarDesign_Container_MainLbl").css("color", "#" + value);
    }
    else {
        $("#AvatarDesign_Container_Lbl").css("color", "#f0f0f0");
        $("#AvatarDesign_Container_MainLbl").css("color", "#f0f0f0");
    }
});

$("#AddTextToTheTextEditor_Btn").on("click", function () {
    let type = $("#TextType_Val").val();
    let pasteTo = $("#PasteTo_Val").val();
    let widgetId = $("#WidgetId_Val").val();
    let preparedText1 = $("#PreparedText1_Val").val();
    let preparedText2 = $("#PreparedText2_Val").val();

    if (preparedText1 != undefined && preparedText2 != undefined) {
        textEditor(type + "-TextEditorStyle", preparedText1, preparedText2, pasteTo);
        animatedOpen(false, widgetId, true, true);
    }
});

$("#ColorPickerVia_Hex").on("click", function () {
    $('#MainColorChangeForm_Box').slideDown(400);
    $('#RGBColorChange_Box').slideUp(400);
    $(this).addClass('btn-primary');
    $(this).removeClass('btn-light');
    $('#ColorPickerVia_RGB').addClass('btn-light');
    $('#ColorPickerVia_RGB').removeClass('btn-primary');
});
$("#ColorPickerVia_RGB").on("click", function () {
    $('#MainColorChangeForm_Box').slideUp(400);
    $('#RGBColorChange_Box').slideDown(400);
    $(this).addClass('btn-primary');
    $(this).removeClass('btn-light');
    $('#ColorPickerVia_Hex').addClass('btn-light');
    $('#ColorPickerVia_Hex').removeClass('btn-primary');
});

$(document).on("change", ".color-picker-range", function (event) {
    let colorValArr = [];
    let classValues = document.getElementsByClassName("color-picker-range");

    for (let i = 0; i < classValues.length; i++) {
        let value = document.getElementById(classValues[i].id).value;
        if (value != undefined) colorValArr.push(value);
        else colorValArr.push(0);
    }

    let red = colorValArr[0];
    let green = colorValArr[1];
    let blue = colorValArr[2];
    let rgbVal_Text = "rgb(" + red + ", " + green + ", " + blue + ")";

    let hexValue = rgbToHex(parseInt(red), parseInt(green), parseInt(blue));
    $("#RGBSelectedColor_Box").css("background-color", rgbVal_Text);
    $("#RGB_SelectedColor_InRGB_Lbl").text(rgbVal_Text);
    $("#RGB_SelectedColor_InHEX_Lbl").text(hexValue);
});
$(document).on("change", ".form-range", function (event) {
    $("#" + event.target.id + "-Span").text($(this).val());
});

$(document).on("click", ".text-decoder", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let pasteTo = $("#" + event.target.id).attr("data-paste-to");
        $("#" + trueId).val(setBackAllUsersInText($("#" + trueId).val()));
        $("#" + trueId).val(replaceAllUsersInText($("#" + trueId).val()));
        textDecoder($("#" + trueId).val(), pasteTo);
    }
});
$(document).on("click", ".text-editor", function (event) {
    let elementId = getTrueId(event.target.id);
    let pasteTo = $("#" + event.target.id).attr("data-bs-paste-to");

    if (elementId != undefined && pasteTo != undefined) {
        textEditor(elementId, null, null, pasteTo);
    }
});

function getTrueId(name) {
    let trueId = name.substring(0, name.lastIndexOf("-"));
    if (trueId != null || trueId != undefined) return trueId;
    else return null;
}

function openSidebar() {
    if (fullWidth < 768) {
        $("#Main_SideBar").fadeIn(250);
        $("#Main_SideBar").css("left", 0);
    }
}

function closeSidebar() {
    if (fullWidth < 768) {
        setTimeout(function () {
            $("#Main_SideBar").fadeOut(350);
            $("#Main_SideBar").css("left", "-1200px");
        }, 550);
    }
}

function valueToHex(value) {
    let hexValue = value.toString(16);
    return hexValue.length == 1 ? "0" + hexValue : hexValue;
}

function rgbToHex(red, green, blue) {
    return valueToHex(red) + valueToHex(green) + valueToHex(blue);
}

function textEditor(elementId, preparedText1, preparedText2, pasteTo) {
    let element = parseInt(elementId);
    let cursorStartPos = $("#" + pasteTo).prop("selectionStart");
    let cursorEndPos = $("#" + pasteTo).prop("selectionEnd");
    let selectedText = null;
    let fullText = $("#" + pasteTo).val();
    let part1 = fullText.substring(0, cursorStartPos);
    let part2;

    part1 = fullText.substring(0, cursorStartPos);
    if (cursorStartPos == cursorEndPos) {
        part2 = fullText.substring(cursorStartPos, fullText.length);
    }
    else {
        selectedText = fullText.substring(cursorStartPos, cursorEndPos - 1);
        part2 = fullText.substring(cursorEndPos, fullText.length);
    }

    switch (element) {
        case 0:
            if (selectedText == null) {
                fullText = part1 + "[[]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[[" + selectedText + "]] " + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 1:
            if (selectedText == null) {
                fullText = part1 + "[{]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[{" + selectedText + "]] " + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 2:
            if (selectedText == null) {
                fullText = part1 + "[_]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[_" + selectedText + "]] " + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 3:
            if (selectedText == null) {
                fullText = part1 + "[$]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[$" + selectedText + "]] " + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 4:
            if (selectedText == null) {
                fullText = part1 + "[[ [!='color:" + preparedText1 + "'>" + preparedText2 + "]] " + part2;
                cursorStartPos = part1.length + 14 + preparedText1.length + preparedText2.length;
            }
            else {
                fullText = part1 + "[[ [!='color:" + preparedText1 + "' " + selectedText + "]] " + part2;
                cursorStartPos = part1.length + 14 + part2.length;
            }
            break;
        case 5:
            if (selectedText == null) {
                fullText = part1 + "[# !$='" + preparedText1 + "'>" + preparedText2 + "]]" + part2;
                cursorStartPos = part1.length + 7 + preparedText1.length + preparedText2.length;
            }
            else {
                fullText = part1 + "[# !$='" + preparedText1 + "'>" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 7 + part2.length;
            }
            break;
        case 6:
            if (selectedText == null) {
                fullText = part1 + "!%='" + preparedText1 + "'>" + preparedText2 + "]]" + part2;
                cursorStartPos = part1.length + 7 + preparedText1.length + preparedText2.length;
            }
            else {
                fullText = part1 + "[# !%='" + preparedText1 + "'>" + selectedText + "!]" + part2;
                cursorStartPos = part1.length + 7 + part2.length;
            }
            break;
        default:
            break;
    }

    $("#" + pasteTo).val(fullText);
    $("#" + pasteTo).prop("selectionStart", cursorStartPos);
    $("#" + pasteTo).prop("selectionEnd", cursorStartPos);
    $("#" + pasteTo).focus();
}

function textDecoder(text, setIn) {
    text = text.replaceAll("[#", "<span>");
    text = text.replace("[[", "<span class='fw-500'>");
    text = text.replaceAll("[{", "<span class='fst-italic'>");
    text = text.replaceAll("[_", "<span class='text-decoration-underline'>");
    text = text.replaceAll("[$", "<span class='style-font'>");
    text = text.replaceAll("[!", "<span style");
    text = text.replaceAll("!$", "<a class='text-decoration-none text-primary' href");
    text = text.replaceAll("!%", "<span class='card-text info-popover' data-bs-toggle='popover' data-bs-custom-class='tooltip-asset-one shadow-sm p-0' data-bs-placement='top' data-bs-container='body' data-bs-content");
    text = text.replaceAll("]]", "</span>");
    text = text.replaceAll("*]", "</span>");
    text = text.replaceAll("[*", "<span class='user-via-shortname'>");
    text = text.replaceAll("!]", "</button>");

    $("#" + setIn).html(text);
}

function setBackAllUsersInText(text) {
    text = text.replaceAll("[*", "");
    text = text.replaceAll("*]", "");

    return text;
}

function replaceAllUsersInText(text) {
    let usersArray = [];
    if (text != null) {
        for (let i = 0; i < text.length; i++) {
            if (text[i] == "@") {
                usersArray.push(text.substring(i, text.indexOf(" ", i)));
            }
        }

        if (usersArray.length > 0) {
            for (let i = 0; i < usersArray.length; i++) {
                text = text.replaceAll(usersArray[i], "[*" + usersArray[i] + "*]");
            }
        }
        return text;
    }
    return null;
}

function findAllUsersFromText(text) {
    let atsArray = [];
    let shortnamesArray = [];

    if (text != null) {
        for (let i = 0; i < text.length; i++) {
            if (text[i] == "@") {
                atsArray.push(i);
            }
        }
    }

    if (atsArray.length > 0) {
        for (let i = 0; i < atsArray.length; i++) {
            let shortname = text.substring(atsArray[i], text.indexOf(" ", atsArray[i] + 1));
            if (shortname != null) shortnamesArray.push(shortname);
        }
        return shortnamesArray;
    }
    return null;
}

function additionalBtnSelector(width) {
    if (width < 768) {
        $("#FirstReserve_Btn").html(' <i class="fa-solid fa-bars text-muted"></i> <br/>Menu');
        $("#FirstReserve_Btn").addClass("open-sidebar");
        $("#FirstMain_Btn").fadeOut(0);
        $("#FirstReserve_Btn").fadeIn(0);

    }
}

function slide(forAll, element) {
    if (!forAll) {
        $(".slider-container").css("margin-left", "32px");
        setTimeout(function () {
            $(".slider-container").css("margin-left", "-6000px");
            $(".slider-container").fadeOut(350);
        }, 350);
        setTimeout(function () {
            $("#" + element).fadeIn(0);
        }, 700);
        setTimeout(function () {
            $("#" + element).css("margin-left", "40px");
        }, 750);
        setTimeout(function () {
            $("#" + element).css("margin-left", 0);
        }, 1200);
    }

    //if (forAll) {
    //    $(".slider-container").fadeIn(0);
    //    $(".slider-container").css("margin-left", "16px");
    //    setTimeout(function () {
    //        $(".slider-container").css("margin-left", 0);
    //    }, 350);
    //}
    //else {
    //    $(".slider-container").css("margin-left", "16px");
    //    setTimeout(function () {
    //        $(".slider-container").css("margin-left", "-1200px");
    //        $(".slider-container").fadeOut(0);
    //    }, 350);
    //    setTimeout(function () {
    //        $("#" + element).fadeIn(0);
    //        $("#" + element).css("margin-left", "16px");
    //        setTimeout(function () {
    //            $("#" + element).css("margin-left", 0);
    //        }, 100);
    //    }, 350);
    //}
}

function animatedOpen(forAll, element, sticky, closeAllOtherContainers) {
    if (forAll) {
        let botOffNavbarAdditionalValue = botOffNavbarH;
        if ($("#" + element).hasClass("smallside-box-container")) {
            botOffNavbarH = 0;
        }

        if (!closeAllOtherContainers) {
            $("." + element).fadeIn(300);
            if (sticky) {
                $("." + element).css("bottom", botOffNavbarH + 18 + "px");
                setTimeout(function () {
                    $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                    botOffNavbarH = botOffNavbarAdditionalValue;
                }, 650);
            }
            else {
                $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                botOffNavbarH = botOffNavbarAdditionalValue;
            }
        }
        else {
            animatedClose(true, "main-container", true, true);
            setTimeout(function () {
                $("." + element).fadeIn(300);
                if (sticky) {
                    $("." + element).css("bottom", botOffNavbarH + 18 + "px");
                    setTimeout(function () {
                        $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                    }, 650);
                }
                else {
                    $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                }
            }, 350);
        }
    }
    else {
        let botOffNavbarAdditionalValue = botOffNavbarH;
        if ($("#" + element).hasClass("smallside-box-container")) {
            botOffNavbarH = 0;
        }

        if (!closeAllOtherContainers) {
            $("#" + element).fadeIn(300);
            if (sticky) {
                $("#" + element).css("bottom", botOffNavbarH + 18 + "px");
                setTimeout(function () {
                    $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                    botOffNavbarH = botOffNavbarAdditionalValue;
                }, 650);
            }
            else {
                $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                botOffNavbarH = botOffNavbarAdditionalValue;
            }
        }
        else {
            animatedClose(true, "main-container", true, true);
            setTimeout(function () {
                $("#" + element).fadeIn(300);
                if (sticky) {
                    $("#" + element).css("bottom", botOffNavbarH + 18 + "px");
                    setTimeout(function () {
                        $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                    }, 650);
                }
                else {
                    $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                }
            }, 350);
        }
    }
}

function animatedClose(forAll, element, alternativeAnima, speedUp) {
    let timeOutDuration = speedUp ? 170 : 340;

    if (forAll) {
        if (alternativeAnima) {
            $("." + element).css("bottom", "17px");
            setTimeout(function () {
                $("." + element).css("bottom", "-1200px");
                $("." + element).fadeOut(350);
            }, timeOutDuration);
        }
        else {
            $("." + element).css("bottom", botOffNavbarH + 42 + "px");
            setTimeout(function () {
                $("." + element).css("bottom", "-1200px");
                $("." + element).fadeOut(350);
            }, timeOutDuration);
        }
    }
    else {
        if (alternativeAnima) {
            $("#" + element).css("bottom", "17px");
            setTimeout(function () {
                $("#" + element).css("bottom", "-1200px");
                $("#" + element).fadeOut(350);
            }, timeOutDuration);
        }
        else {
            $("#" + element).css("bottom", botOffNavbarH  + 42 + "px");
            setTimeout(function () {
                $("#" + element).css("bottom", "-1200px");
                $("#" + element).fadeOut(350);
            }, timeOutDuration);
        }
    }
}

function alert(icon, text, btn1Text, btn2Text, btn1Action, btn2Action, btn1WhatToDo, btn2WhatToDo, duration) {
    if (icon == null) $("#Alert_Container-Icon").html(" <i class='fa-regular fa-bell text-neon-purple'></i> ");
    else $("#Alert_Container-Icon").html(" " + icon + " ");

    if (text != null) $("#Alert_Container-Text").html(text);
    else $("#Alert_Container-Text").html("No specific text for this alert. Just enjoy it");

    if (btn1Text != null) $("#Alert_Container-Btn1").html(btn1Text);
    else $("#Alert_Container-Btn1").html("Done");
    if (btn2Text != null) $("#Alert_Container-Btn2").html(btn2Text);
    else {
        $("#Alert_Container-Btn2").fadeOut(100);
        $("#Alert_Container-Btn2").html("Done");
    }

    $("#Alert_Container").fadeIn(100);
    $("#Alert_Container").css("top", "12px");
    setTimeout(function () {
        $("#Alert_Container").css("top", "-9px");
    }, 370);
    setTimeout(function () {
        $("#Alert_Container").css("top", 0);
    }, 630);

    if (btn1Action != null) {
        switch (btn1Action) {
            case 0:
                $("#Alert_Container-Btn1").on("click", function () {
                    $("#Alert_Container").css("top", "12px");
                    setTimeout(function () {
                        $("#Alert_Container").css("top", "-1200px");
                        $("#Alert_Container").fadeOut(350);
                    }, 350);
                });
                break;
            default:
                $("#Alert_Container-Btn1").on("click", function () {
                    $("#Alert_Container").css("top", "12px");
                    setTimeout(function () {
                        $("#Alert_Container").css("top", "-1200px");
                        $("#Alert_Container").fadeOut(350);
                    }, 350);
                });
                break;
        }
    }
    else {
        $("#Alert_Container-Btn1").on("click", function () {
            $("#Alert_Container").css("top", "12px");
            setTimeout(function () {
                $("#Alert_Container").css("top", "-1200px");
                $("#Alert_Container").fadeOut(350);
            }, 350);
        });
    }

    if (btn2Action != null) {
        switch (btn2Action) {
            case 0:
                $("#Alert_Container-Btn2").on("click", function () {
                    $("#Alert_Container").css("top", "12px");
                    setTimeout(function () {
                        $("#Alert_Container").css("top", "-1200px");
                        $("#Alert_Container").fadeOut(350);
                    }, 350);
                });
                break;
            default:
                $("#Alert_Container-Btn2").on("click", function () {
                    $("#Alert_Container").css("top", "12px");
                    setTimeout(function () {
                        $("#Alert_Container").css("top", "-1200px");
                        $("#Alert_Container").fadeOut(350);
                    }, 350);
                });
                break;
        }
    }
    else {
        $("#Alert_Container-Btn2").on("click", function () {
            $("#Alert_Container").css("top", "12px");
            setTimeout(function () {
                $("#Alert_Container").css("top", "-1200px");
                $("#Alert_Container").fadeOut(350);
            }, 350);
        });
    }

    clearTimeout(alertTimeout);
    if (duration != 0 || duration != null) {
        alertTimeout = setTimeout(function () {
            $("#Alert_Container").css("top", "12px");
            setTimeout(function () {
                $("#Alert_Container").css("top", "-1200px");
                $("#Alert_Container").fadeOut(350);
            }, 350);
        }, (duration + 1) * 1000);
    }
    else {
        alertTimeout = setTimeout(function () {
            $("#Alert_Container").css("top", "12px");
            setTimeout(function () {
                $("#Alert_Container").css("top", "-1200px");
                $("#Alert_Container").fadeOut(350);
            }, 350);
        }, Infinity);
    }
} 

function displayCorrect(width) {
    let neededH = fullHeight - 24 - botOffNavbarH;
    let sideBarStatus = $("#Main_SideBar").css("margin-left");
    sideBarStatus = sideBarStatus == undefined ? -1200 : parseInt(sideBarStatus);

    $(".main-container").css("max-height", neededH + "px");
    $(".smallside-box-container").css("max-height", neededH + "px");

    if (sideBarStatus < 0) {
        $("#MainBotOffNavbar").css("width", "100%");
        $(".main-container").css("width", "100%");
        $(".main-container").css("left", 0);
    }
    else {
        if (width < 768) {
            $("#MainBotOffNavbar").css("width", "100%");
            $("#MainBotOffNavbar").css("left", 0);
            $("#Main_SideBar").css("left", "-1200px");
            $("#Main_SideBar").css("width", "100%");
            $("#Main_SideBar").fadeOut(0);

            $(".main-container").css("width", "100%");
            $(".main-container").css("left", 0);
            $(".smallside-box-container").css("width", "100%");
            $(".smallside-box-container").css("left", 0);
            //$(".smallside-modal-container").css("left", "2.4%");
            //$(".smallside-modal-container").css("width", "95%");
        }
        else {
            $("#Main_SideBar").css("width", "35%");
            let leftBarW = $("#Main_SideBar").innerWidth() + 3;
            let leftW = fullWidth - leftBarW - 3;
            let smallSideContainerW = leftBarW - 5;

            $("#MainBotOffNavbar").css("width", leftW + "px");
            $("#MainBotOffNavbar_Box").css("width", leftW + "px");
            $("#MainBotOffNavbar").css("left", leftBarW + "px");

            $("#Main_SideBar").fadeIn(0);
            $("#Main_SideBar").css("left", 0);
            $(".main-container").css("width", leftW + "px");
            $(".main-container").css("left", leftBarW + "px");
            $(".smallside-box-container").css("width", smallSideContainerW + 2 + "px");
            //$(".smallside-modal-container").css("left", "10px");
            //$(".smallside-modal-container").css("width", smallSideContainerW - 19 + "px");
        }
    }
}

$(document).on("mouseover", ".info-popover", function () {
    $(this).popover("show");
});
$(document).on("mouseout", ".info-popover", function () {
    $(this).popover("hide");
});