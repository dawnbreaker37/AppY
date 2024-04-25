﻿let fullWidth = 0;
let fullHeight = 0;
let botOffNavbarH = 0;
let alertTimeout;
let currentUrl;
let batteryInfo;
let letters = /^[A-Za-z]+$/;

window.onload = function () {
    let standardTimeout = 300;
    batteryInfo = navigator.getBattery;
    currentUrl = document.location.href;
    fullHeight = parseInt(window.innerHeight);
    fullWidth = parseInt(window.innerWidth);
    if ($("#AvatarSticker_Hdn_Val").val() != "") {
        $(".unpictured-container-label").html($("#AvatarSticker_Hdn_Val").val());
        $(".unpictured-container-label-sm").html($("#AvatarSticker_Hdn_Val").val());
    }

    setTimeout(function () {
        displayCorrect(fullWidth);
        additionalBtnSelector(fullWidth);

        if (fullWidth >= 768) {
            animatedOpen(false, "SmallsidePreloaded_Container", true, false);
            standardTimeout += 350;
            setTimeout(function () {
                animatedOpen(false, "Preloaded_Container", true, false);
            }, 350);
        }
        else animatedOpen(false, "Preloaded_Container", true, false);
        $("#MainTopOffNavbar").slideDown(300);
    }, 300);

    if (currentUrl.toLowerCase().includes("/discuss") || currentUrl.toLowerCase().includes("/chat/c")) {
        botNavbarClose(true, null);
        insideBoxOpen("PreloadedInside_Box", true);
        botNavbarOpen("Preloaded_BotOffNavbar");
        statusSlider("StatusBar_Lbl", 2);
        replaceAllTheTextInMessages();

        setTimeout(function () {
            slideToTheBottom("Preloaded_Container");
        }, standardTimeout);
    }
    else if (currentUrl.toLowerCase().includes("/page")) {
        statusSlider("StatusBar_Lbl", 2);
        botNavbarOpen("MainBotOffNavbar", null, null);
    }
    else {
        botNavbarOpen("MainBotOffNavbar", null, null);
    }

    getBatteryLevel(true);
}
window.onresize = function () {
    fullHeight = parseInt(window.innerHeight);
    fullWidth = parseInt(window.innerWidth);
    displayCorrect(fullWidth);
    additionalBtnSelector(fullWidth);
}
$(window).on("blur", function () {
    $("#SetLastSeen_Form").submit();
});

$(batteryInfo).on("chargingchange levelchange", function () {
    getBatteryLevel(true);
});

$("#SetLastSeen_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
    });
});

$("#CheckAccountCredentials_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            if (currentUrl.toLowerCase().includes("/linkedaccounts")) {
                $("#LinkAccountInfo_Span").text(response.account);
                $("#LinkAccountInfo2_Span").text(response.account);
                $("#SARC_Id_Val").val(response.id);
                $("#SRCVE_Email_Val").val(response.account);
                $("#LinkAccount_Box-NextSlide").click();
            }
            alert('<i class="fa-solid fa-check-double fa-bounce text-primary" style="--fa-animation-duration: 1.75s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
        else {
            if (currentUrl.toLowerCase().includes("/linkedaccounts")) {
                $("#Email").val("");
                $("#Password").val("");
            }
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
    });
});

$("#LinkAccounts_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let linkedAccountsCount = parseInt($("#LinkedAccountsCount_Val").val());

            let div = $("<div class='box-container bg-light p-2 mt-1 mb-1'></div>");
            let accountName = $("<span class='h6'></span>");
            let separatorDiv = $("<div></div>");
            let lastSeenInfo = $("<small class='card-text text-muted'></small>");
            let enterToAcc_Btn = $("<button type='button' class='btn btn-sm btn-standard text-primary float-end ms-1 easy-entry-to-acc'>Enter</button>");
            accountName.html(response.result.pseudoName);
            lastSeenInfo.html("last seen " + dateAndTimeTranslator(response.result.lastSeen));
            enterToAcc_Btn.attr("id", response.result.id + "-EasyEntryToAcc_Btn");
            div.attr("id", response.result.id + "-LinkedAccount_Box");
            div.append(enterToAcc_Btn);
            div.append(accountName);
            div.append(separatorDiv);
            div.append(lastSeenInfo);

            if (linkedAccountsCount <= 0) {
                $("#LinkedAccounts_Box").removeClass("text-center");
                $("#LinkedAccounts_Box").empty();
                $("#LinkedAccounts_Box").append(div);
            }
            else {
                $("#LinkedAccounts_Box").append(div);
            }

            linkedAccountsCount++;
            $("#LinkedAccountsCount_Val").val(linkedAccountsCount);
            $("#LinkedAccountsCount_Span").text(linkedAccountsCount);

            $("#ALA_Id_Val").val(0);
            $("#SARC_Id_Val").val(0);
            $("#ReserveCode").val("");
            $("#Email").val("");
            $("#Password").val("");
            $("#LinkAccount_Box-PrevSlide").click();
            setTimeout(function () {
                $("#LinkAccount_Box-PrevSlide").click();
            }, 350);
            alert('<i class="fa-solid fa-check-double fa-bounce text-primary" style="--fa-animation-duration: 1.75s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
    });
});
$("#UnlinkAccounts_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let linkedAccountsCount = parseInt($("#LinkedAccountsCount_Val").val());
            linkedAccountsCount--;
            $("#LinkedAccountsCount_Span").text(linkedAccountsCount);
            $("#LinkedAccountsCount_Val").val(linkedAccountsCount);

            slideToLeft(response.id + "-LinkedAccount_Box");
            setTimeout(function () {
                $("#" + response.id + "-LinkedAccount_Box").fadeOut(0);
            }, 350);
            setTimeout(function () {
                $("#" + response.id + "-LinkedAccount_Box").empty();
                if (linkedAccountsCount <= 0) {
                    $("#LinkedAccounts_Box").empty();
                    $("#LinkedAccounts_Box").addClass("text-center");
                    let icon = $('<h2 class="h2"> <i class="fa-solid fa-link-slash"></i> </h2>');
                    let title = $('<h4 class="h4">No Linked Accounts Yet</h4>');
                    let description = $('<small class="card-text text-muted">link any other your account (if you got another one) to this to make switching between them much easier and quickier</small>');
                    $("#LinkedAccounts_Box").append(icon);
                    $("#LinkedAccounts_Box").append(title);
                    $("#LinkedAccounts_Box").append(description);
                }
            }, 375);
            alert('<i class="fa-solid fa-check-double fa-bounce text-primary" style="--fa-animation-duration: 1.75s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
        }
    });
});

$(document).on("click", ".easy-entry-to-acc", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#EasyEntry_Id_Val").val(trueId);
        $("#EasyEntry_Form").submit();
    }
    else {
        $("#EasyEntry_Id_Val").val(0);
    }
});
$(document).on("click", ".delete-easy-entry", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#UnlinkingAccount_Id_Val").val(trueId);
        $("#UnlinkAccounts_Form").submit();
    }
    else {
        $("#UnlinkingAccount_Id_Val").val(0);
    }
});
$(document).on("click", "#SendReserveCodeMediated_Btn", function () {
    let emailValue = $("#SRCVE_Email_Val").val();
    if (emailValue != "") {
        $("#SendReserveCodeViaEmail_Form").submit();
    }
});
$("#SendReserveCodeViaEmail_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#SendReserveCodeMediated_Btn").attr("disabled", true);
            alert('<i class="fa-regular fa-envelope fa-beat-fade text-primary"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
        }
    });
});

$("#SubmitAccountReserveCode_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#ALA_Id_Val").val(response.id);
            $("#LinkAccount_Box-NextSlide").click();
        }
        else {
            $("#ReserveCode").val("");
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
    });
});

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

$("#EditBatterySettings_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            if (response.result == 0) {
                $("#EcoModeOnAt_Lbl").text("turned off");
                $("#BatteryLevelIndicator_Span").html(' <i class="fas fa-battery-empty"></i> ' + response.result + "%");
                $("#BatteryLevelIndicator_Span").fadeOut(300);
            }
            else if (response.result > 20 && response.result <= 50) {
                $("#EcoModeOnAt_Lbl").text("turn on when battery level is below then");
                $("#BatteryLevelIndicator_Span").html(' <i class="fas fa-battery-half"></i> ' + response.result + "%");
            }
            else if (response.result > 50 && response.result <= 75) {
                $("#EcoModeOnAt_Lbl").text("turn on when battery level is below then");
                $("#BatteryLevelIndicator_Span").html(' <i class="fas fa-battery-three-quarters"></i> ' + response.result + "%");
            }
            else if (response.result > 75) {
                $("#EcoModeOnAt_Lbl").text("turn on when battery level is below then");
                $("#BatteryLevelIndicator_Span").html(' <i class="fas fa-battery-full"></i> ' + response.result + "%");
            }
            else {
                $("#EcoModeOnAt_Lbl").text("turn on when battery level is below then");
                $("#BatteryLevelIndicator_Span").html(' <i class="fas fa-battery-quarter text-danger"></i> ' + response.result + "%");
            }
            alert('<i class="fa-solid fa-check-circle text-primary"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
        }
    });
});

$("#SetProfilePhoto_Form").on("submit", function (event) {
    event.preventDefault();

    let files = $("#SelectaFile_Val").get(0).files;
    let url = $(this).attr("action");
    let formData = new FormData();
    formData.append("id", $("#SetProfilePhoto_Id_Val").val());
    formData.append("file", files[0]);

    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.success) {
                animatedClose(false, "ProfilePhoto_Container", true, true);
                $("#AvatarUrl_Id").attr("src", "/Avatars/" + response.result);
                $("#AvatarNone_Box").slideUp(400);
                $("#Avatar_Box").slideDown(400);
                setTimeout(function () {
                    $("#AvatarTestUrl_Val").attr("src", "/Avatars/" + response.result);
                    $("#DeleteAvatar_Box").fadeIn(250);
                    $("#PicturedAvatar_Box").fadeIn(250);
                    $("#ProfilePhoto-Header").fadeOut(250);
                }, 400);
                closeSidebar();
            }
            else {
                alert('<i class="fa-regular fa-circle-xmark fa-shake fa-flip-horizontal text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
            }
        }
    });
});
$("#DeleteProfilePhoto_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#Avatar_Box").slideUp(400);
            $("#AvatarNone_Box").slideDown(400);
            animatedClose(false, "ProfilePhoto_Container", true, true);
            setTimeout(function () {
                $("#AvatarUrl_Id").attr("src", "#");
                $("#AvatarTestUrl_Val").attr("src", "#");
                $("#DeleteAvatar_Box").fadeOut(250);
                $("#PicturedAvatar_Box").fadeOut(250);
                $("#ProfilePhoto-Header").fadeIn(250);
            }, 400);
            closeSidebar();
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake fa-flip-horizontal text-danger"></i>', response.alert, "Okay", null, 0, null, null, null, 4.5);
        }
    });
});
$("#EditPrivacySettings_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-solid fa-lock fa-flip text-primary" style="--fa-animation-duration: 1.25s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 3.5);
            animatedClose(false, "Privacy_Container", true, true);
            setTimeout(function () {
                closeSidebar();
            }, 350);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 3.25);
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

            if(response.sticker != null) $("#UnpicturedAvatar_Lbl").html(" " + response.sticker + " ");
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

$("#FindUserById_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            headerReturn("Preloaded_Container_ShowBox");
            setTimeout(function () {
                $("#UserVeryShortInfo_Box").empty();
                let avatarDiv = $("<div></div>");
                let pseudoName = $("<span class='h4'></span>");
                let shortName = $("<small class='card-text text-muted'></small>");
                let separatorZero = $("<div></div>");
                let separatorOne = $("<div class='mt-2'></div>");
                let pageLinkBtn = $("<a href='#' class='btn btn-standard-asset bg-white btn-sm'></a>");

                if (response.result.avatarUrl == null) {
                    let unpicturedAvatarDiv = $("<div class='unpictured-container-label-md'></div>");
                    avatarDiv.addClass("image-picture-none-container-md text-center bg-white mx-auto");
                    unpicturedAvatarDiv.html(response.result.pseudoName[0]);
                    avatarDiv.append(unpicturedAvatarDiv);
                }
                else {
                    avatarDiv.addClass("image-container-md mx-auto text-center");
                    let picturedAvatarImg = $("<img class='image-container-md' alt='Cannot display this image' />");
                    picturedAvatarImg.attr("src", "/Avatars/" + response.result.avatarUrl);
                    avatarDiv.append(picturedAvatarImg);
                }
                pseudoName.html(response.result.pseudoName);
                shortName.html("@" + response.result.shortName);
                pageLinkBtn.html(' <i class="fa-solid fa-up-right-from-square text-primary"></i> ' + response.result.pseudoName + "'s Page");
                pageLinkBtn.attr("href", "/User/Page/" + response.result.shortName);

                $("#UserVeryShortInfo_Box").append(avatarDiv);
                $("#UserVeryShortInfo_Box").append(pseudoName);
                $("#UserVeryShortInfo_Box").append(separatorZero);
                $("#UserVeryShortInfo_Box").append(shortName);
                $("#UserVeryShortInfo_Box").append(separatorOne);
                $("#UserVeryShortInfo_Box").append(pageLinkBtn);

                insideBoxOpen("UserInfo_Box");
            }, 250);
        }
        else alert('<i class="fa-solid fa-user-slash"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
    });
});
$("#FindUserByShortname_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#UserVeryShortInfo_Box").empty();
            let avatarDiv = $("<div></div>");
            let pseudoName = $("<span class='h4'></span>");
            let shortName = $("<small class='card-text text-muted'></small>");
            let separatorZero = $("<div></div>");
            let separatorOne = $("<div class='mt-2'></div>");
            let pageLinkBtn = $("<a href='#' class='btn btn-standard-asset bg-white btn-sm'></a>");

            if (response.result.avatarUrl == null) {
                let unpicturedAvatarDiv = $("<div class='unpictured-container-label-md'></div>");
                avatarDiv.addClass("image-picture-none-container-md text-center bg-white mx-auto");
                unpicturedAvatarDiv.html(response.result.pseudoName[0]);
                avatarDiv.append(unpicturedAvatarDiv);
            }
            else {
                avatarDiv.addClass("image-container-md mx-auto text-center");
                let picturedAvatarImg = $("<img class='image-container-md' alt='Cannot display this image' />");
                picturedAvatarImg.attr("src", "/Avatars/" + response.result.avatarUrl);
                avatarDiv.append(picturedAvatarImg);
            }
            pseudoName.html(response.result.pseudoName);
            shortName.html("@" + response.result.shortName);
            pageLinkBtn.html(' <i class="fa-solid fa-up-right-from-square text-primary"></i> ' + response.result.pseudoName + "'s Page");
            pageLinkBtn.attr("href", "/User/Page/" + response.result.shortName);

            $("#UserVeryShortInfo_Box").append(avatarDiv);
            $("#UserVeryShortInfo_Box").append(pseudoName);
            $("#UserVeryShortInfo_Box").append(separatorZero);
            $("#UserVeryShortInfo_Box").append(shortName);
            $("#UserVeryShortInfo_Box").append(separatorOne);
            $("#UserVeryShortInfo_Box").append(pageLinkBtn);

            insideBoxOpen("UserInfo_Box");
        }
        else alert('<i class="fa-solid fa-user-slash"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
    });
});

$("#GetNotificationInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let title = $("#" + response.result.id + "-ForAllNotificationTitle").html();
            let isChecked = $("#" + response.result.id + "-ForAllIsChecked_Span").html();
            let urgencyValue = setUrgencyIcon(response.result.notificationCategoryId);
            let isUnkillable = $("#" + response.result.id + "-ForAllUnkillableNotification").html();

            $("#NotificationCategory_Btn").html(" " + urgencyValue + " ");
            $("#NotificationTitle_Lbl").html(title);
            $("#NotificationDescription_Lbl").html(response.result.description);
            if (isUnkillable != "") {
                $("#IsUnkillable_Btn").fadeIn(300);
            }
            else $("#IsUnkillable_Btn").fadeOut(300);

            if (isChecked) $("#NotificationOtherInfo_Lbl").html("<span class='text-primary'> " + isChecked + " </span> <span class='text-muted'>" + dateAndTimeTranslator(response.result.sentAt) + "</span>");
            else $("#NotificationOtherInfo_Lbl").html("<span class='text-muted'> " + isChecked + " </span> <span class='text-muted'>" + dateAndTimeTranslator(response.result.sentAt) + "</span>");

            openSidebar();
            animatedOpen(false, "NotificationInfo_Container", true, false);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#MarkTheNotificationAsRead_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-Dropdown_ForAll_NotificationMarkAsRead").fadeOut(250);
            $("#" + response.id + "-Dropdown_ForChecked_NotificationMarkAsRead").fadeOut(250);
            $("#" + response.id + "-Dropdown_ForMissed_NotificationMarkAsRead").fadeOut(250);
            $("#" + response.id + "-ForAllIsChecked_Span").html("<i class='fa-solid fa-check-double text-primary'></i> ");
            $("#" + response.id + "-ForMissed_IsChecked_Span").html("<i class='fa-solid fa-check-double text-primary'></i> ");
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#PinTheNotification_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-Dropdown_ForCheckedPinTheNotification").fadeOut(300);
            $("#" + response.id + "-Dropdown_ForMissedPinTheNotification").fadeOut(300);
            $("#" + response.id + "-Dropdown_ForAllPinTheNotification").fadeOut(300);
            $("#" + response.id + "-Dropdown_ForCheckedUnpinTheNotification").fadeIn(300);
            $("#" + response.id + "-Dropdown_ForMissedUnpinTheNotification").fadeIn(300);
            $("#" + response.id + "-Dropdown_ForAllUnpinTheNotification").fadeIn(300);

            $("#" + response.id + "-ForAll_IsPinned").fadeIn(250);
            $("#" + response.id + "-ForMissed_IsPinned").fadeIn(250);
            $("#" + response.id + "-ForChecked_IsPinned").fadeIn(250);
            alert('<i class="fa-solid fa-thumbtack fa-bounce text-primary"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
        }
        else {
            alert('<i class="fa-solid fa-exclamation text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#UnpinTheNotification_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-Dropdown_ForCheckedPinTheNotification").fadeIn(300);
            $("#" + response.id + "-Dropdown_ForMissedPinTheNotification").fadeIn(300);
            $("#" + response.id + "-Dropdown_ForAllPinTheNotification").fadeIn(300);
            $("#" + response.id + "-Dropdown_ForCheckedUnpinTheNotification").fadeOut(300);
            $("#" + response.id + "-Dropdown_ForMissedUnpinTheNotification").fadeOut(300);
            $("#" + response.id + "-Dropdown_ForAllUnpinTheNotification").fadeOut(300);

            $("#" + response.id + "-ForAll_IsPinned").fadeOut(250);
            $("#" + response.id + "-ForMissed_IsPinned").fadeOut(250);
            $("#" + response.id + "-ForChecked_IsPinned").fadeOut(250);
            alert("<i class='fa-solid fa-slash text-danger'>", response.alert, "Done", null, 0, null, null, null, 3.75);
        }
        else {
            alert('<i class="fa-solid fa-exclamation text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});
$("#DeleteNotification_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let currentCount = parseInt($("#NotificationCount_Span").text());
            let missedNotificationsCount = parseInt($("#MissedNotificationsCount_BtnSpan").text());
            let checkedNotificationsCount = parseInt($("#CheckedNotificationsCount_BtnSpan").text());
            currentCount--;

            if (missedNotificationsCount == 1) {
                missedNotificationsCount--;
                let noMoreNotifications_ForMissed_Div = $('<div class="box-container text-center mt-2" style="display: none;"></div>');
                let icon_ForMissed = $('<h2 class= "h2"> <i class="fa-regular fa-circle-check"></i> </h2>');
                let title_ForMissed = $('<h5 class="h5 style-font">No Missed Notifications</h5>');
                let description_ForMissed = $('<small class="card-text text-muted">You have checked your notifications</small>');

                noMoreNotifications_ForMissed_Div.append(icon_ForMissed);
                noMoreNotifications_ForMissed_Div.append(title_ForMissed);
                noMoreNotifications_ForMissed_Div.append(description_ForMissed);

                slideToLeft(response.id + "-ForMissed_Notification_Box");
                setTimeout(function () {
                    $("#ForMissedNotifications_Box").append(noMoreNotifications_ForMissed_Div);
                    noMoreNotifications_ForMissed_Div.fadeIn(300);
                }, 1050);
            }
            else {
                slideToLeft(response.id + "-ForMissed_Notification_Box");
            }

            if (checkedNotificationsCount == 1) {
                checkedNotificationsCount--;
                let noMoreNotifications_ForChecked_Div = $('<div class="box-container text-center mt-2" style="display: none;"></div>');
                let icon_ForChecked = $('<h2 class= "h2"> <i class="fa-regular fa-bell"></i> </h2>');
                let title_ForChecked = $('<h5 class="h5 style-font">No Checked Notifications</h5>');
                let description_ForChecked = $('<small class="card-text text-muted">You have not checked any notification. Mark them as read from your missed list to appear them here</small>');

                noMoreNotifications_ForChecked_Div.append(icon_ForChecked);
                noMoreNotifications_ForChecked_Div.append(title_ForChecked);
                noMoreNotifications_ForChecked_Div.append(description_ForChecked);

                slideToLeft(response.id + "-ForChecked_Notification_Box");
                setTimeout(function () {
                    $("#ForCheckedNotifications_Box").append(noMoreNotifications_ForChecked_Div);
                    noMoreNotifications_ForChecked_Div.fadeIn(300);
                }, 1050);
            }
            else {
                slideToLeft(response.id + "-ForChecked_Notification_Box");
            }

            if (currentCount <= 0) {
                let noMoreNotifications_Div = $('<div class="box-container text-center mt-2" style="display: none;"></div>');
                let icon = $('<h2 class= "h2"> <i class="fa-regular fa-circle-check"></i> </h2>');
                let title = $('<h5 class="h5 style-font">No Missed Notifications</h5>');
                let description = $('<small class="card-text text-muted">You have checked all your notifications</small>');

                noMoreNotifications_Div.append(icon);
                noMoreNotifications_Div.append(title);
                noMoreNotifications_Div.append(description);

                slideToLeft(response.id + "-ForAll_Notification_Box");
                setTimeout(function () {
                    $("#ForAllNotifications_Box").append(noMoreNotifications_Div);
                    noMoreNotifications_Div.fadeIn(300);
                }, 1050);

                $("#NotificationCount_Span").text(currentCount);
                $("#MissedNotificationsCount_BtnSpan").text(missedNotificationsCount);
                $("#AllNotificationsCount_BtnSpan").text(currentCount);
                $("#CheckedNotificationsCount_BtnSpan").text(checkedNotificationsCount);
            }
            else {
                //slideToLeft(response.id + "-" + ForAll_Notification_Box);
                $("#" + response.id + "-ForAll_Notification_Box").slideUp(350);
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});

//Chats start//
$("#ChatShortname_Val").on("change", function (event) {
    let value = $(this).val();
    if (value != null && value.length <= 24) {
        $("#CSA_Value_Val").val(value);
        $("#ChatShortnameAvailabilityCheck_Form").submit();
    }
    else {
        $("#EditChatSbmt_Btn").attr("disabled", false);
        $("#CSA_Value_Val").val("");
    }
});
$("#ChatShortnameAvailabilityCheck_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("EditChatSbmt_Btn").attr("disabled", false);
            $("#ShortnameDescription_Lbl").html("<span class='text-primary'>Shortname is free to use</span>");
        }
        else {
            $("#EditChatSbmt_Btn").attr("disabled", true);
            $("#ShortnameDescription_Lbl").html("<span class='text-danger'>Shortname has been already taken</span>");
        }

        setTimeout(function () {
            $("#ShortnameDescription_Lbl").text("Shortname must be unique. May container up to 24 characters with these criteries: [A-Z, a-z], [0-9] and underscore");
        }, 4500);
    });
});

$("#MuteTheChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-ChatMuted_Icon").slideDown(250);
            if (currentUrl.toLowerCase().includes("/chat/c")) $("#" + response.id + "-ChatMuteOrUnmute").html(" <i class='fa-regular fa-bell text-primary'></i> <br/>Unmute");
            else $("#" + response.id + "-ChatMuteOrUnmute").html(" <i class='fa-regular fa-bell'></i> Unmute");
            $("#" + response.id + "-ChatMuteOrUnmute").removeClass("mute-the-chat");
            $("#" + response.id + "-ChatMuteOrUnmute").addClass("unmute-the-chat");
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
    });
});
$("#UnmuteTheChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-ChatMuted_Icon").slideUp(250);
            if (currentUrl.toLowerCase().includes("/chat/c")) $("#" + response.id + "-ChatMuteOrUnmute").html(" <i class='fa-regular fa-bell-slash text-primary'></i> <br/>Mute");
            else $("#" + response.id + "-ChatMuteOrUnmute").html(" <i class='fa-regular fa-bell-slash'></i> Unmute");
            $("#" + response.id + "-ChatMuteOrUnmute").addClass("mute-the-chat");
            $("#" + response.id + "-ChatMuteOrUnmute").removeClass("unmute-the-chat");
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
    });
});
$("#PinTheChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-ChatPinned_Icon").slideDown(250);
            $("#" + response.id + "-ChatPinOrUnpin").removeClass("pin-the-chat");
            $("#" + response.id + "-ChatPinOrUnpin").addClass("unpin-the-chat");
            $("#" + response.id + "-ChatPinOrUnpin").html(' <i class="fa-solid fa-link-slash"></i> Unpin');
            alert('<i class="fa-solid fa-thumbtack fa-bounce text-primary" style="--fa-animation-duration: 1.75s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
        }
    });
});
$("#UnpinTheChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-ChatPinned_Icon").slideUp(250);
            $("#" + response.id + "-ChatPinOrUnpin").removeClass("unpin-the-chat");
            $("#" + response.id + "-ChatPinOrUnpin").addClass("pin-the-chat");
            $("#" + response.id + "-ChatPinOrUnpin").html(' <i class="fa-solid fa-thumbtack"></i> Pin');
            alert('<i class="fa-solid fa-link-slash"></i>', response.alert, "Done", null, 0, null, null, null, 3.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
        }
    });
});
$("#RestoreTheChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            document.location.href = "/Chat/C/" + response.result;
        }
        else alert('<i class="fa-regular fa-circle-xmark text-danger fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 3;"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
    });
});
$(document).on("click", ".mute-the-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId > 0) {
        $("#MuteTheChat_Id_Val").val(trueId);
        $("#MuteTheChat_Form").submit();
    }
    else {
        $("#MuteTheChat_Id_Val").val(0);
    }
});
$(document).on("click", ".unmute-the-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId > 0) {
        $("#UnmuteTheChat_Id_Val").val(trueId);
        $("#UnmuteTheChat_Form").submit();
    }
    else {
        $("#UnmuteTheChat_Id_Val").val(0);
    }
});
$(document).on("click", ".pin-the-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#PinTheChat_Id_Val").val(trueId);
        $("#PinTheChat_Form").submit();
    }
    else {
        $("#PinTheChat_Id_Val").val(0);
    }
});
$(document).on("click", ".unpin-the-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#UnpinTheChat_Id_Val").val(trueId);
        $("#UnpinTheChat_Form").submit();
    }
    else {
        $("#UnpinTheChat_Id_Val").val(0);
    }
});
$(document).on("click", ".restore-the-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#RestoreTheChat_Id_Val").val(trueId);
        $("#RestoreTheChat_Form").submit();
    }
    else {
        $("#RestoreTheChat_Id_Val").val(0);
    }
});
$("#GetChats_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#Chats_Container-Box").empty();
            $("#ChatsCount_Span").text(response.count);
            animatedClose(true, "smallside-box-container", true, true);
            if (response.count > 0) {
                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let avatarDiv = $("<div class='image-picture-none-container-sm text-center bg-white d-inline-block me-2'></div>");
                    let name = $("<span class='h5 text-truncate'></span>");
                    let separatorZero = $("<div></div>");
                    let lastMessage = $("<small class='card-text ms-2'>No sent messages</small>");
                    let isMutedIcon = $('<small class="card-text text-orange float-end me-2 mt-1" style="display: none;"> <i class="fa-regular fa-bell-slash"></i> </small>');
                    let isPinnedIcon = $("<small class='card-text float-end me-2 mt-1' style='display: none;'> <i class='fa-solid fa-thumbtack'></i> </small>");
                    let noAvatarLbl = $("<div class='unpictured-container-label-sm text-dark'></div>");

                    let dropdownDiv = $("<div class='dropdown'></div>");
                    let dropdownBtn = $("<button type='button' class='btn btn-standard btn-sm float-end ms-2' data-bs-toggle='dropdown' aria-expanded='false'> <i class='fa-solid fa-ellipsis-h'></i> </button>");
                    let dropdownUl = $("<ul class='dropdown-menu shadow-sm p-1'></ul>");
                    let dropdownLi0 = $("<li class='text-center mb-1'></li>");
                    let dropdownLi1 = $("<li></li>");
                    let dropdownLi2 = $("<li></li>");
                    let dropdownLi3 = $("<li></li>");
                    let dropdownLi4 = $("<li></li>");
                    let dropdownHeader = $("<small class='card-text text-muted'></small>");
                    let dropdownBtn1 = $("<a href='#' class='dropdown-item mb-1'> <i class='fa-solid fa-up-right-from-square'></i> Chat</a>");
                    let dropdownBtn2 = $("<button type='button' class='dropdown-item mb-1'></button>");
                    let dropdownBtn3 = $("<button type='button' class='dropdown-item'></button>");
                    let dropdownBtn4 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    dropdownLi0.append(dropdownHeader);
                    dropdownLi1.append(dropdownBtn1);
                    dropdownLi2.append(dropdownBtn2);
                    dropdownLi3.append(dropdownBtn3);
                    dropdownLi4.append(dropdownBtn4);
                    dropdownUl.append(dropdownLi0);
                    dropdownUl.append(dropdownLi1);
                    dropdownUl.append(dropdownLi2);
                    dropdownUl.append(dropdownLi3);
                    dropdownUl.append(dropdownLi4);
                    dropdownDiv.append(dropdownBtn);
                    dropdownDiv.append(dropdownUl);

                    name.attr("id", response.result[index].chatId + "-ChatName_Lbl");
                    dropdownBtn2.attr("id", response.result[index].chatId + "-ChatPinOrUnpin");
                    dropdownBtn3.attr("id", response.result[index].chatId + "-ChatMuteOrUnmute");
                    isPinnedIcon.attr("id", response.result[index].chatId + "-ChatPinned_Icon");
                    isMutedIcon.attr("id", response.result[index].chatId + "-ChatMuted_Icon");

                    if (response.result[index].chatName != null) {
                        if (response.result[index].chatName.length > 24) {
                            name.html(response.result[index].chatName.substring(0, 24) + "...");
                        }
                        else name.html(response.result[index].chatName);
                        noAvatarLbl.html(response.result[index].chatName[0]);
                    }
                    else {
                        name.html("New Chat");
                        noAvatarLbl.html("N");
                    }
                    avatarDiv.append(noAvatarLbl);

                    if (response.result[index].isPinned) {
                        isPinnedIcon.fadeIn(0);
                        dropdownBtn2.addClass("unpin-the-chat");
                        dropdownBtn2.html(' <i class="fa-solid fa-link-slash"></i> Unpin');
                    }
                    else {
                        dropdownBtn2.addClass("pin-the-chat");
                        dropdownBtn2.html(' <i class="fa-solid fa-thumbtack"></i> Pin');
                    }
                    if (response.result[index].isMuted) {
                        isMutedIcon.fadeIn(0);
                        dropdownBtn3.addClass("unmute-the-chat");
                        dropdownBtn3.html(' <i class="fa-regular fa-bell"></i> Unmute');
                    }
                    else {
                        dropdownBtn3.addClass("mute-the-chat");
                        dropdownBtn3.html(' <i class="fa-regular fa-bell-slash"></i> Mute');
                    }

                    if (response.result[index].deletedAt != null) {
                        div.css("border", "1px solid red");
                        dropdownLi1.fadeOut(0);
                        dropdownHeader.html("deleted " + dateAndTimeTranslator(response.result[index].deletedAt));
                        dropdownBtn4.addClass("text-danger");
                        dropdownBtn4.addClass("restore-the-chat");
                        dropdownBtn4.attr("id", response.result[index].chatId + "-RestoreTheChat");
                        dropdownBtn4.html(' <i class="fa-solid fa-trash-arrow-up"></i> Restore');
                        lastMessage.html("<small class='badge bg-danger rounded-pill text text-light'>Deleted</small> No Sent Messages");
                    }
                    else {
                        dropdownBtn1.attr("href", "/Chat/C/" + response.result[index].chatId);
                        name.addClass("relocate-to-chat");
                        dropdownLi4.fadeOut(0);
                        dropdownLi0.fadeOut(0);
                    }

                    if (!currentUrl.toLowerCase().includes("/discuss") && !currentUrl.toLowerCase().includes("/chat")) {
                        div.append(dropdownDiv);
                    }
                    div.append(isPinnedIcon);
                    div.append(isMutedIcon);
                    div.append(avatarDiv);
                    div.append(name);
                    div.append(separatorZero);
                    div.append(lastMessage);

                    $("#Chats_Container-Box").append(div);
                });
            }
            else {
                let div = $("<div class='box-container bg-light text-center p-2'></div>");
                let icon = $("<h2 class='h2'> <i class='fa-solid fa-comment-slash'></i> </h2>");
                let title = $("<h5 class='h5'>No Chats</h5>");
                let description = $("<small class='card-text text-muted'>You haven't got any chat yet</small>");

                div.append(icon);
                div.append(title);
                div.append(description);
                $("#Chats_Container-Box").append(div);
            }

            setTimeout(function () {
                openSidebar();
                animatedOpen(false, "Chats_Container", true, false);
            }, 350);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake"></i>', response.alert, "Close", null, 0, null, null, null, 3.25);
        }
    });
});
$("#GetChatsShortly_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#ChatsCount_Span").text(response.count);
            if (response.count > 0) {
                let canBeForwarded;
                if (response.isForForwarding) {
                    canBeForwarded = "message can be forwarded";
                }
                else canBeForwarded = "message cannot be forwarded";
                $("#Chats_Container-Box").empty();

                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let h6 = $("<span class='h6'></span>");
                    let status = $("<small class='card-text text-muted'></small>");
                    let separatorZero = $("<div></div>");
                    let separatorOne = $("<div class='mt-2'></div>");
                    let forwardBtn = $("<button type='button' class='btn btn-standard btn-sm text-primary forward-chat-msg'> <i class='fa-solid fa-share'></i> Forward</button>");

                    if (response.result[index].chatName != null) h6.html(response.result[index].chatName);
                    else h6.html("New Chat");
                    status.text(canBeForwarded);
                    div.append(h6);
                    div.append(separatorZero);
                    div.append(status);
                    if (response.isForForwarding) {
                        div.append(separatorOne);
                        div.append(forwardBtn);
                    }

                    $("#Chats_Container-Box").append(div);
                });

                openSidebar();
                animatedClose(true, "smallside-box-container", true, true);
                setTimeout(function () {
                    animatedOpen(false, "Chats_Container", false, false);
                }, 325);

                setTimeout(function () {
                    insideBoxClose(true, null);
                }, 200);
            }
            else alert('<i class="fa-regular fa-circle-xmark fa-shake"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
    });
});
$("#EditChat_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {

        }
        else {
        }
    });
});
$("#ClearChatHistory_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {

        }
        else {

        }
    });
});
//Chats ending//

$("#CreateDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            if (response.isPrivate) {
                alert('<i class="fa-regular fa-circle-check text-neon-purple fa-shake"></i>', "Discussion has been successfully created. Since this discussion is a private discussion here's the password for it: <span class='fw-500'>" + response.password + "</span>", "Go to Discussion", "Close", 1, 0, "/discussion/discuss/" + response.result, null, 7.5);
            }
            else document.location.href = "/discussion/discuss/" + response.result;
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger"></i>', response.alert, "Close", null, 0, null, null, null, 5);
        }
    });
});
$("#DeleteTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#Preloaded_Container").css("pointer-events", "none");
            $("#Preloaded_Container").css("filter", "blur(9px)");
            alert('<i class="fa-regular fa-trash-can"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});
$("#RestoreTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let currentCount = parseInt($("#DiscussionsCount_Span").text());
            currentCount--;

            alert('<i class="fa-solid fa-arrow-rotate-right fa-spin text-neon-purple" style="--fa-animation-duration: 0.9s; --fa-animation-iteration-count: 2;"></i>', response.alert, "Close", "Go to Discussion", 0, 1, null, "/Discussion/Discuss/" + response.id, 4.75);
            if (currentCount <= 0) {
                $("#DiscussionsCount_Span").text("no deleted");
                $("#GetDeletedDiscussions_Form_Box").fadeOut(350);
                slideToLeft(response.id + "-DeletedDiscussionBox");

                let div = $("<div class='box-container p-2 text-center'></div>");
                let icon = $('<h1 class="h1"> <i class="fa-solid fa-xmark"></i> </h1>');
                let title = $("<h3 class='h3'>No Deleted Discussions</h3>");
                let description = $("<small class='card-text text-muted'>You have restored all your deleted discussions</small>");

                setTimeout(function () {
                    animatedClose(false, "Discussions_Container", true, true);
                }, 400);
                setTimeout(function () {
                    div.append(icon);
                    div.append(title);
                    div.append(description);
                    $("#Discussions_Container-Box").append(div);
                    animatedOpen(false, "Discussions_Container", true, false);
                }, 900);
            }
            else {
                $("#DiscussionsCount_Span").text(currentCount);
                slideToLeft(response.id + "-DeletedDiscussionBox");
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});

$("#GetDiscussionShortInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let div = $("<div class='bordered-container p-2'></div>");
            let title = $("<span class='h6'></span>");
            let separatorZero = $("<div></div>");
            let separatorOne = $("<div class='mt-2'></div>");
            let shortLink = $("<small class='card-text text-muted'></small>")
            let description = $("<small class='card-text white-space-on'></small>");

            title.html(response.result.name);
            if (response.result.description != null) description.html(response.result.description);
            else description.text("No description");
            shortLink.text("@" + response.result.shortlink);
            div.append(title);
            div.append(separatorZero);
            div.append(shortLink);
            div.append(separatorOne);
            div.append(description);

            alert('<i class="fa-solid fa-circle-info text-primary fa-beat" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', div, ' <i class="fa-solid fa-arrow-up-right-from-square"></i> To the Discussion', ' <i class="fa-solid fa-xmark"></i> Close', 1, 0, "/Discussion/Discuss/" + response.result.id, null, 450);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

$("#FindDiscussions_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        $("#FoundResults_Box").empty();
        if (response.success) {
            if (response.count > 0) {
                let countLbl = $("<h6 class='h6'></h6>");
                countLbl.text(response.count + " discussion(s) found");
                $("#FoundResults_Box").append(countLbl);
                $.each(response.result, function (index) {
                    let aboutIt_Btn = $("<button type='button' class='btn btn-standard btn-sm float-end ms-1 btn-get-discussion-info'>About</button>");
                    let div = $("<div class='bordered-container mt-2 p-2'></div>");
                    let name = $("<h6 class='h6 text-truncate'></h6>");
                    let createdAt = $("<small class='card-text text-muted'></small>");

                    name.html(response.result[index].discussionName);
                    createdAt.text("created " + dateAndTimeTranslator(response.result[index].createdAt));
                    aboutIt_Btn.attr("id", response.result[index].id + "-GetDiscussionInfo_Btn");

                    div.append(aboutIt_Btn);
                    div.append(name);
                    div.append(createdAt);

                    $("#FoundResults_Box").append(div);
                });
            }
            else {
                let div = $("<div class='box-container text-center'></div>");
                let description = $("<p class='card-text text-muted'></p>");
                description.html(' <i class="fa-solid fa-magnifying-glass"></i> ' + response.result);
                div.append(description);

                $("#FoundResults_Box").append(div);
            }
        }
        else {
            let div = $("<div class='box-container text-center'></div>");
            let description = $("<p class='card-text text-muted'></p>");
            description.html(' <i class="fa-solid fa-magnifying-glass"></i> ' + response.result);
            div.append(description);

            $("#FoundResults_Box").append(div);
        }
    });
});

$("#GetOtherUsersDiscussions_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#DiscussionsInfo_Box").empty();
            $("#SameDiscussionsInfo_Box").empty();
            $.each(response.result, function (index) {
                let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                let avatarDiv = $("<div></div>");
                let name = $("<span class='h5 relocate-to-discussion'></span>");
                let separatorZero = $("<div></div>");
                let lastMessage = $("<small class='card-text'></small>");
                let isPrivateIcon = $('<small class="card-text text-orange float-end ms-2 mt-1" style="display: none;"> <i class="fa-solid fa-lock"></i> </small>');
                let joinedAndCreated = $("<small class='card-text text-muted'></small>");

                name.html(response.result[index].discussionName);
                joinedAndCreated.text("joined at " + dateAndTimeTranslator(response.result[index].joinedAt));
                if (response.result[index].isPrivate) isPrivateIcon.fadeIn(300);
                if (response.result[index].discussionAvatar == null) {
                    let noAvatarLbl = $("<div class='unpictured-container-label-sm text-dark'></div>");
                    avatarDiv.addClass("image-picture-none-container-sm text-center bg-white d-inline-block me-2");
                    noAvatarLbl.html(response.result[index].discussionName[0]);
                    avatarDiv.append(noAvatarLbl);
                }
                else {
                    let avatarImg = $("<img class='image-container-sm' alt='Cannot display this image' />");
                    avatarDiv.addClass("image-container-sm text-center d-inline-block me-2");
                    avatarImg.attr("src", "/DiscussionAvatars/" + response.result[index].discussionAvatar);
                    avatarDiv.append(avatarImg);
                }
                name.attr("id", response.result[index].discussionId + "-RelocateToTheDiscussion_Span");

                div.append(isPrivateIcon);
                div.append(avatarDiv);
                div.append(name);
                div.append(lastMessage);
                div.append(separatorZero);
                div.append(joinedAndCreated);

                $("#DiscussionsInfo_Box").append(div);
            });

            if (response.similarsCount > 0) {
                let similarsHeader = $("<small class='card-text text-muted'></span>");
                $("#SameDiscussionsCount_Span").fadeIn(300);
                $("#SameDiscussionsInfo_Box").fadeIn(300);
                if (response.similarsCount == 1) {
                    $("#SameDiscussionsCount_Span").text(" ∙ single similar discussion");
                    similarsHeader.text("You both got one similar discussion");
                }
                else {
                    $("#SameDiscussionsCount_Span").text(" ∙ " + response.similarsCount + " similar discussions");
                    similarsHeader.text("You both got " + response.similarsCount + " similar discussion");
                }
                $("#SameDiscussionsInfo_Box").append(similarsHeader);
                $.each(response.similars, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let name = $("<span class='h5 relocate-to-discussion'></span>");
                    let separatorZero = $("<div></div>");
                    let joinedAndCreated = $("<small class='card-text text-muted'></small>");

                    name.html(response.similars[index].discussionName);
                    joinedAndCreated.text("joined at " + dateAndTimeTranslator(response.similars[index].joinedAt));
                    name.attr("id", response.similars[index].discussionId + "-RelocateToTheSimilarDiscussion_Span");

                    div.append(name);
                    div.append(separatorZero);
                    div.append(joinedAndCreated);

                    $("#SameDiscussionsInfo_Box").append(div);
                });
            }
            else {
                $("#SameDiscussionsInfo_Box").fadeOut(300);
            }

            $("#GetOtherUsersDiscussionsSbmt_Btn").attr("disabled", true);
            slideToRight("Discussions_Container");
        }
        else {
            alert('<i class="fa-solid fa-quote-right fa-shake" style="--fa-animation-duration: 1.1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
    });
});

$("#GetDiscussions_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        $("#Discussions_Container-Box").empty();
        if (response.success) {
            $("#Discussions_Container_Header").text("Discussions");
            $("#DiscussionsCount_Span").text(response.count);
            if (response.count > 0) {
                let tooltipReminder = $("<div class='box-container bg-light p-1'></div>");
                let tolltipReminderText = $("<span class='h6'> <i class='fa-solid fa-tag text-primary'></i> Tap on discussion's name to open it</span>");
                let tooltipDivider = $("<div></div>");
                let tooltipSmallReminder = $("<small class='card-text text-muted'>Tap on others icon to edit some settings</small>");
                tooltipReminder.append(tolltipReminderText);
                tooltipReminder.append(tooltipDivider);
                tooltipReminder.append(tooltipSmallReminder);
                $("#Discussions_Container-Box").append(tooltipReminder);

                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let avatarDiv = $("<div></div>");
                    //let noAvatarDiv = $("<div class='image-picture-none-container-sm text-center bg-dark text-light d-inline me-2'></div>");
                    let name = $("<span class='h5 relocate-to-discussion'></span>");
                    let separatorZero = $("<div></div>");
                    let lastMessage = $("<small class='card-text'></small>");
                    let joinedAndCreated = $("<small class='card-text text-muted'></small>");
                    let isMutedIcon = $('<small class="card-text text-orange float-end me-2 mt-1"> <i class="fa-regular fa-bell-slash"></i> </small>');
                    let isPinnedIcon = $("<small class='card-text float-end me-2 mt-1'> <i class='fa-solid fa-thumbtack'></i> </small>");

                    let dropdownDiv = $("<div class='dropdown'></div>");
                    let dropdownBtn = $("<button type='button' class='btn btn-standard btn-sm float-end ms-2' data-bs-toggle='dropdown' aria-expanded='false'> <i class='fa-solid fa-ellipsis-h'></i> </button>");
                    let dropdownUl = $("<ul class='dropdown-menu shadow-sm p-1'></ul>");
                    let dropdownLi1 = $("<li></li>");
                    let dropdownLi2 = $("<li></li>");
                    let dropdownLi3 = $("<li></li>");
                    let dropdownHeader = $("<small class='card-text text-muted fw-500'></small>");
                    let dropdownBtn1 = $("<a href='#' class='dropdown-item'> <i class='fa-solid fa-up-right-from-square'></i> Discussion</a>");
                    let dropdownBtn2 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    let dropdownBtn3 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    dropdownLi1.append(dropdownBtn1);
                    dropdownLi2.append(dropdownBtn2);
                    dropdownLi3.append(dropdownBtn3);
                    dropdownUl.append(dropdownLi1);
                    dropdownUl.append(dropdownLi2);
                    dropdownUl.append(dropdownLi3);
                    dropdownDiv.append(dropdownBtn);
                    dropdownDiv.append(dropdownUl);

                    isMutedIcon.attr("id", response.result[index].discussionId + "-MutedIcon");
                    isPinnedIcon.attr("id", response.result[index].discussionId + "-PinnedIcon");
                    name.attr("id", response.result[index].discussionId + "-Name_Lbl_RelocateToDiscussion");
                    joinedAndCreated.attr("id", response.result[index].discussionId + "-DateNTime_Lbl_RelocateToDiscussion");
                    name.html(response.result[index].discussionName);
                    joinedAndCreated.html("joined " + dateAndTimeTranslator(response.result[index].joinedAt));
                    dropdownHeader.text("joined " + dateAndTimeTranslator(response.result[index].joinedAt));
                    dropdownBtn1.attr("href", "/Discussion/Discuss/" + response.result[index].discussionId);
                    dropdownBtn2.attr("id", response.result[index].discussionId + "-MuteOrUnmuteTheDiscussion");
                    dropdownBtn3.attr("id", response.result[index].discussionId + "-PinOrUnpinTheDiscussion");

                    if (response.result[index].discussionAvatar == null) {
                        let noAvatarLbl = $("<div class='unpictured-container-label-sm text-dark'></div>");
                        avatarDiv.addClass("image-picture-none-container-sm text-center bg-white d-inline-block me-2");
                        noAvatarLbl.html(response.result[index].discussionName[0]);
                        avatarDiv.append(noAvatarLbl);
                    }
                    else {
                        let avatarImg = $("<img class='image-container-sm' alt='Cannot display this image' />");
                        avatarDiv.addClass("image-container-sm text-center d-inline-block me-2");
                        avatarImg.attr("src", "/DiscussionAvatars/" + response.result[index].discussionAvatar); 
                        avatarDiv.append(avatarImg);
                    }
                    if (response.result[index].lastMessageText != null) {
                        lastMessage.text(response.result[index].lastMessageText);
                    }
                    else lastMessage.fadeOut(0);


                    if (!response.result[index].isMuted) {
                        isMutedIcon.fadeOut(350);
                        dropdownBtn2.html(" <i class='fa-regular fa-bell-slash'></i> Mute");
                        dropdownBtn2.addClass("mute-the-discussion");
                    }
                    else {
                        dropdownBtn2.html(" <i class='fa-regular fa-bell'></i> Unmute");
                        dropdownBtn2.addClass("unmute-the-discussion");
                    }

                    if (response.result[index].isPinned) {
                        dropdownBtn3.addClass("unpin-the-discussion");
                        dropdownBtn3.html(' <i class="fa-solid fa-link-slash"></i> Unpin');
                    }
                    else {
                        isPinnedIcon.fadeOut(350);
                        dropdownBtn3.addClass("pin-the-discussion");
                        dropdownBtn3.html(' <i class="fa-solid fa-thumbtack"></i> Pin');
                    }

                    if (!currentUrl.toLowerCase().includes("/discuss") && !currentUrl.toLowerCase().includes("/chat")) {
                        div.append(dropdownDiv);
                    }
                    div.append(isPinnedIcon);
                    div.append(isMutedIcon);
                    div.append(avatarDiv);
                    div.append(name);
                    div.append(separatorZero);
                    div.append(lastMessage);
                    div.append(joinedAndCreated);

                    $("#Discussions_Container-Box").append(div);
                });
            }
            else {
                let div = $("<div class='box-container p-2 text-center'></div>");
                let icon = $('<h2 class="h2"> <i class="fa-regular fa-xmark-circle"></i> </h2>');
                let title = $("<h6 class='h6'>No Discussions</h6>");
                let description = $("<small class='card-text text-muted'>you haven't got any discussions yet</small>");

                div.append(icon);
                div.append(title);
                div.append(description);

                $("#Discussions_Container-Box").append(div);
            }
        }
        else {
            let div = $("<div class='box-container p-2'></div>");
            let icon = $('<h2 class="h2"> <i class="fa-solid fa-xmark"></i> </h2>');
            let title = $("<h6 class='h6'>No Discussions</h6>");
            let description = $("<small class='card-text text-muted'>you haven't got any discussions yet</small>");

            div.append(icon);
            div.append(title);
            div.append(description);

            $("#Discussions_Container-Box").append(div);
        }

        openSidebar();
        animatedClose(true, "smallside-box-container", true, true);
        setTimeout(function () {
            animatedOpen(false, "Discussions_Container", true, false);
        }, 300);
    });
});
$("#GetDeletedDiscussions_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#Discussions_Container-Box").empty();
            if (response.count > 0) {
                $("#Discussions_Container_Header").text("Deleted Discussions");
                $("#DiscussionsCount_Span").text(response.count);
                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let name = $("<span class='h6'></span>");
                    let separatorZero = $("<div></div>");
                    let joinedAndDeleted = $("<small class='card-text text-muted'></small>");
                    let separatorOne = $("<div class='mt-2'></div>");
                    let selectToRestore = $('<button type="button" class="btn btn-standard btn-sm select-to-restore-the-discussion"> <i class="fa-solid fa-arrow-rotate-right"></i> Restore</button>')

                    div.attr("id", response.result[index].discussionId + "-DeletedDiscussionBox");

                    name.html(response.result[index].discussionName);
                    if (response.result[index].deletedAt != null) joinedAndDeleted.html("created " + dateAndTimeTranslator(response.result[index].createdAt) + ", deleted " + dateAndTimeTranslator(response.result[index].deletedAt));
                    else joinedAndDeleted.html("created " + dateAndTimeTranslator(response.result[index].createdAt) + ", deleted recently");
                    selectToRestore.attr("id", response.result[index].discussionId + "-SelectToRestoreTheDiscussion");

                    div.append(name);
                    div.append(separatorZero);
                    div.append(joinedAndDeleted);
                    div.append(separatorOne);
                    div.append(selectToRestore);

                    $("#Discussions_Container-Box").append(div);
                });

                openSidebar();
                animatedOpen(false, "Discussions_Container", true, false);
            }
            else alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
        else alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
    });
});
$("#GetMembersInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#MembersInfo_Container").empty();
            $("#GetMembersInfo_SbmtBtn").attr("disabled", true);
            if (response.count > 0) {
                if (response.count == 1) $("#MembersCount_Lbl").text(response.count + " member");
                else $("#MembersCount_Lbl").text(response.count + " members");

                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let memberAccessBadge = $("<small type='button' class='card-text style-font ms-2'></small>");
                    let userName = $("<span class='h6 user-via-id'></span>");
                    let separator = $("<div></div>");
                    let joinedAt_Small = $("<small class='card-text text-muted'></small>");

                    let dropdownDiv = $("<div class='dropdown'></div>");
                    let dropdownBtn = $("<button type='button' class='btn btn-standard btn-sm float-end ms-2' data-bs-toggle='dropdown' aria-expanded='false'> <i class='fa-solid fa-ellipsis-h'></i> </button>");
                    let dropdownUl = $("<ul class='dropdown-menu p-1'></ul>");
                    let dropdownLi0 = $("<li class='text-center'></li>");
                    let dropdownLi1 = $("<li></li>");
                    let dropdownLi2 = $("<li></li>");
                    let dropdownLi3 = $("<li></li>");
                    let dropdownLi4 = $("<li></li>");
                    let dropdownHeader = $("<small class='card-text text-muted fw-500'></small>");
                    let dropdownBtn1 = $("<a href='#' class='dropdown-item'></a>");
                    let dropdownBtn3 = $("<button type='button' class='dropdown-item mt-1 text-danger'> <i class='fas fa-user-times'></i> Delete</button>");
                    let dropdownBtn2 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    let dropdownBtn4 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    dropdownLi0.append(dropdownHeader);
                    dropdownLi1.append(dropdownBtn1);
                    dropdownLi2.append(dropdownBtn2);
                    dropdownLi3.append(dropdownBtn3);
                    dropdownLi4.append(dropdownBtn4);
                    dropdownUl.append(dropdownLi0);
                    dropdownUl.append(dropdownLi1);
                    dropdownUl.append(dropdownLi2);
                    dropdownUl.append(dropdownLi4);
                    dropdownUl.append(dropdownLi3);
                    dropdownDiv.append(dropdownBtn);
                    dropdownDiv.append(dropdownUl);

                    div.attr("id", response.result[index].userId + "-Member_Container");
                    userName.html(response.result[index].userName);
                    userName.attr("id", response.result[index].userId + "-GetUserInfoViaId");
                    joinedAt_Small.attr("id", response.result[index].userId + "-JoinedAt_Small");
                    dropdownBtn1.html(" <i class='fa-solid fa-up-right-from-square'></i> " + response.result[index].userName + "'s Page");
                    memberAccessBadge.attr("id", response.result[index].userId + "-MemberInfoAccessLevel");
                    dropdownBtn1.attr("href", "/User/Page/" + response.result[index].userId);
                    dropdownBtn4.attr("id", response.result[index].userId + "-BlockTheUser");

                    if (response.result[index].isBlocked) {
                        div.css("border", "1px solid orange");
                        joinedAt_Small.html("<span class='badge bg-orange text-light style-font p-1'>Blocked</span> joined " + dateAndTimeTranslator(response.result[index].joinedAt));
                    }
                    else joinedAt_Small.html("joined " + dateAndTimeTranslator(response.result[index].joinedAt));

                    if (response.currentUserAccessLevel > response.result[index].accessLevel) {
                        dropdownBtn2.attr("id", response.result[index].userId + "-BtnEditAccessLevel");
                        dropdownBtn2.attr("data-bs-title", response.currentUserId);
                        dropdownBtn2.addClass("btn-edit-access-level");
                        dropdownBtn2.fadeIn(0);

                        dropdownBtn3.fadeIn(0);
                        dropdownBtn3.addClass("btn-delete-user-from-discussion");
                        dropdownBtn3.attr("id", response.result[index].userId + "-DeleteUserFromDiscussion");

                        if (response.result[index].accessLevel == 0) dropdownBtn2.html(' <i class="fa-solid fa-user-tag"></i> Set as Admin');
                        else dropdownBtn2.html(' <i class="fa-regular fa-circle-user"></i> Set as Standard');

                        if (!response.result[index].isBlocked) {
                            dropdownBtn4.fadeIn(0);
                            dropdownBtn4.addClass("btn-block-the-user-out-from-the-discussion");
                            dropdownBtn4.html(' <i class="fa-solid fa-user-minus"></i> Block');
                        }
                        else {
                            dropdownBtn4.fadeIn(0);
                            dropdownBtn2.fadeOut(0);
                            dropdownBtn3.fadeOut(0);
                            dropdownBtn4.addClass("btn-unblock-the-user-out-from-the-discussion");
                            dropdownBtn4.html(' <i class="fa-solid fa-user-check"></i> Unlock');
                        }
                    }
                    else {
                        dropdownBtn2.fadeOut(0);
                        dropdownBtn3.fadeOut(0);
                        dropdownBtn4.fadeOut(0);
                    }

                    if (response.result[index].accessLevel == 1) {
                        dropdownBtn2.attr("data-bs-html", 0);
                        memberAccessBadge.addClass("text-danger");
                        memberAccessBadge.text("Admin");
                    }
                    else if (response.result[index].accessLevel == 2) {
                        memberAccessBadge.addClass("text-primary");
                        memberAccessBadge.text("Owner");
                        dropdownBtn2.attr("data-bs-html", 0);
                    }
                    else {
                        memberAccessBadge.fadeOut(350);
                        dropdownBtn2.attr("data-bs-html", 1);
                    }
                    div.append(dropdownDiv);
                    div.append(userName);
                    div.append(memberAccessBadge);
                    div.append(separator);
                    div.append(joinedAt_Small);

                    $("#EAL_ChangerId_Val").val(response.currentUserId);
                    $("#MembersInfo_Container").append(div);
                });
                slideToRight("MembersInfoMain_Container");
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Done", null, 0, null, null, null, 4);
        }
    });
});

$("#BlockTheUserOutFromTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-circle-check fa-spin fa-spin text-orange" style="--fa-animation-duration: 0.65s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            let membersCount = parseInt($("#MembersCount_Lbl").text());
            if (membersCount <= 1) $("#MembersCount_Lbl").text("1 member");
            else $("#MembersCount_Lbl").text(--membersCount + " members");
            $("#" + response.result + "-Member_Container").css("border", "1px solid orange");
            $("#" + response.result + "-JoinedAt_Small").html("<span class='badge p-1 bg-orange text-light style-font'>Blocked</span>");
            $("#" + response.result + "-BlockTheUser").removeClass("btn-block-the-user-out-from-the-discussion");
            $("#" + response.result + "-BlockTheUser").addClass("btn-unblock-the-user-out-from-the-discussion");
            $("#" + response.result + "-BlockTheUser").html(' <i class="fa-solid fa-user-check"></i> Unblock');
            $("#" + response.result + "-BtnEditAccessLevel").fadeOut(350);
            $("#" + response.result + "-DeleteUserFromDiscussion").fadeOut(350);
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 0.85s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
    });
});
$("#UnblockTheUserOutFromTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-circle-check fa-spin fa-spin text-neon-purple" style="--fa-animation-duration: 0.65s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);

            let membersCount = parseInt($("#MembersCount_Lbl").text());
            $("#MembersCount_Lbl").text(++membersCount + " members");
            $("#" + response.result + "-Member_Container").css("border", "none");
            $("#" + response.result + "-JoinedAt_Small").html("<span class='badge bg-primary text-light p-1 style-font'>Unblocked</span>");
            $("#" + response.result + "-BlockTheUser").addClass("btn-block-the-user-out-from-the-discussion");
            $("#" + response.result + "-BlockTheUser").removeClass("btn-unblock-the-user-out-from-the-discussion");
            $("#" + response.result + "-BlockTheUser").html(' <i class="fa-solid fa-user-minus"></i> Block');
            $("#" + response.result + "-BtnEditAccessLevel").fadeIn(350);
            $("#" + response.result + "-DeleteUserFromDiscussion").fadeIn(350);
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 0.9s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
    });
});

$("#ChangeAccessLevel_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#EAL_UserId_Val").val(0);
            $("#EAL_AccessLevel_Val").val(0);
            animatedClose(false, "EditAccessLevel_Container", true, true);

            $("#" + response.userId + "-BtnEditAccessLevel").attr("data-bs-html", response.result == 0 ? 1 : 0);
            switch (parseInt(response.result)) {
                case 0:
                    $("#" + response.userId + "-MemberInfoAccessLevel").fadeOut(350);
                    $("#" + response.userId + "-BtnEditAccessLevel").html(' <i class="fa-solid fa-user-tag"></i> Set as Admin');
                    break;
                case 1:
                    $("#" + response.userId + "-BtnEditAccessLevel").html(' <i class="fa-regular fa-circle-user"></i> Set as Standard');
                    $("#" + response.userId + "-MemberInfoAccessLevel").addClass("text-danger");
                    $("#" + response.userId + "-MemberInfoAccessLevel").text("Admin");
                    $("#" + response.userId + "-MemberInfoAccessLevel").fadeIn(350);
                    break;
                default:
                    $("#" + response.userId + "-BtnEditAccessLevel").html(' <i class="fa-solid fa-user-tag"></i> Set as Admin');
                    $("#" + response.userId + "-MemberInfoAccessLevel").fadeOut(350);
                    break;
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});

$("#AddMemberToDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-solid fa-plus fa-spin text-primary" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 3.25);
            slideToLeft("MembersInfoMain_Container");
            $("#GetMembersInfo_SbmtBtn").attr("disabled", false);
        }
        else {
            if (parseInt(response.error) == -256) alert('<i class="fa-solid fa-user-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
            else if (parseInt(response.error) == -512) alert('<i class="fa-solid fa-ban text-orange"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
            else if (parseInt(response.error) == -128) alert('<i class="fa-solid fa-user-xmark"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
            else alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
        }
    });
});

$("#JoinToDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            slideToLeft("SendMessage_SecondInside_Box");

            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> <br/>Joined');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", true);
            $("#SendMessage_Text_Val").attr("readonly", false);
            $("#SendMessage_Text_Val").attr("disabled", false);
            $("#LeaveTheDiscussion_SbmtBtn").attr("disabled", false);
            alert('<i class="fa-solid fa-right-to-bracket text-primary"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#JoinToPrivateDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            slideToLeft("SendMessage_SecondInside_Box");

            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> <br/>Joined');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", true);
            $("#SendMessage_Text_Val").attr("readonly", false);
            $("#SendMessage_Text_Val").attr("disabled", false);
            $("#LeaveTheDiscussion_SbmtBtn").attr("disabled", false);

            alert('<i class="fa-solid fa-right-to-bracket text-primary"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#LeaveTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            slideToRight("SendMessage_SecondInside_Box");

            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-solid fa-right-to-bracket"></i> <br/>Join');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", false);
            $("#SendMessage_Text_Val").attr("readonly", true);
            $("#SendMessage_Text_Val").attr("disabled", true);
            $("#SendMessage_SbmtBtn").attr("disabled", true);
            $("#LeaveTheDiscussion_SbmtBtn").attr("disabled", true);
            alert('<i class="fa-solid fa-right-from-bracket"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#DeleteUserFromDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let membersCount = parseInt($("#MembersCount_Lbl").text());
            if (membersCount <= 1) $("#MembersCount_Lbl").text("1 member");
            else $("#MembersCount_Lbl").text(--membersCount + " members");
            slideToLeft(response.result + "-Member_Container");
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 1.25s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
    });
});

$("#EditDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#EditDiscussionInfo_Box").removeClass("show");
            $("#Preloaded_Container_ShowBox-Open").text(response.result.name);
            $("#DiscussionName_Lbl").text(response.result.name);
            $("#AdditionalInfo_DiscussionName_Lbl").text(response.result.name);
            $("#AdditionalInfo_DiscussionShortlink_Lbl").text("@" + response.result.shortlink);
            $("#DiscussionLinkAbout_Span").text("/discussion/discuss" + response.result.shortlink);
            $("#DiscussionShortlink_Lbl").html("@" + response.result.shortlink);
            $("#ShortlinkAdditional_Lbl").html("@<span class='user-select-all'>" + response.result.shortlink + "</span>");
            $("#DiscussionLinkAbout_Val-Copy").text("/Discussion/DiscussionInfo/" + response.result.shortlink);
            if (response.result.description != null) {
                $("#DiscussionDescription_Lbl").html(response.result.description);
            }
            else {
                $("#DiscussionDescription_Lbl").html("No description");
            }
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Done", null, 0, null, null, null, 4.5);
        }
    });
});

$("#SetDiscussionStatus_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            botNavbarClose(false, "StatusEdit_BotOffNavbar");
            setTimeout(function () {
                botNavbarOpen("Preloaded_BotOffNavbar");
            }, 125);
            $("#FifthMain_Btn").fadeOut(300);
            $("#StatusBar_Lbl-2").html(response.result);
            setTimeout(function () {
                $("#SetStatus_Val").val("");
                $("#SetTheStatusSbmt_Btn").attr("disabled", true);
            }, 350);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning" style="--fa-animation-duration: 1.2s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
        }
    });
});

$("#SetDiscussionAvatar_Form").on("submit", function (event) {
    event.preventDefault();

    let imageUrl = $("#SelectaFile_Val").get(0).files;
    let formData = new FormData();
    formData.append("id", $("#SDA_Id_Val").val());
    formData.append("userid", $("#SDA_UserId_Val").val());
    formData.append("image", imageUrl[0]);

    $.ajax({
        type: "POST",
        url: $(this).attr("action"),
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                animatedClose(false, "SetAvatar_Container", true, true);
                setTimeout(function () {
                    $("#UploadedFileName_Lbl").text(response.result);
                    $("#DeleteCurrentAvatar_Box").fadeIn(0);
                }, 350);
                $("#DiscussionAvatar_Img").attr("src", "/DiscussionAvatars/" + response.result);
                $("#UnpicturedAvatar_Box").fadeOut(0);
                $("#PicturedAvatar_Box").fadeIn(300);
                $("#DeleteDiscussionAvatar_SbmtBtn").attr("disabled", false);
            }
            else alert('<i class="fa-regular fa-circle-xmark text-warning fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
        }
    });
});
$("#DeleteDiscussionAvatar_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let firstLetterOfName = $("#DiscussionName_Lbl").text().substring(0, 1);
            animatedClose(false, "SetAvatar_Container", true, true);
            setTimeout(function () {
                $("#UploadedFileName_Lbl").text("Upload Your Image");
                $("#DeleteCurrentAvatar_Box").fadeOut(0);
            }, 350);
            $("#UnpicturedAvatar_Lbl").text(firstLetterOfName);
            $("#DiscussionAvatar_Img").attr("src", "#");
            $("#PicturedAvatar_Box").fadeOut(0);
            $("#UnpicturedAvatar_Box").fadeIn(0);
            $("#DeleteDiscussionAvatar_SbmtBtn").attr("disabled", true);
        }
        else alert('<i class="fa-regular fa-circle-xmark text-warning fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
    });
});

$("#MuteTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-bell-slash fa-shake" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            $("#MuteTheDiscussion_Box").fadeOut(0);
            $("#UnmuteTheDiscussion_Box").fadeIn(0);
            $("#IsMutedHeader_Span").html(' <i class="fa-regular fa-bell-slash"></i> ');
            $("#IsMutedHeader_Span").fadeIn(350);
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").html(" <i class='fa-regular fa-bell-slash'></i> Unmute");
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").removeClass("mute-the-discussion");
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").addClass("unmute-the-discussion");
            $("#" + response.result + "-MutedIcon").slideDown(350);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger fa-shake"></i>', response.alert, "Done", null, 0, null, null, null, 4);
        }
    });
});
$("#UnmuteTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-regular fa-bell fa-shake" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            $("#UnmuteTheDiscussion_Box").fadeOut(0);
            $("#MuteTheDiscussion_Box").fadeIn(0);
            $("#IsMutedHeader_Span").html("");
            $("#IsMutedHeader_Span").fadeOut(350);
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").html(" <i class='fa-regular fa-bell'></i> Mute");
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").addClass("mute-the-discussion");
            $("#" + response.result + "-MuteOrUnmuteTheDiscussion").removeClass("unmute-the-discussion");
            $("#" + response.result + "-MutedIcon").slideUp(350);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-danger fa-shake" style="--fa-animation-duration: 2s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4);
        }
    });
});
$("#PinTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-solid fa-thumbtack fa-bounce" style="--fa-animation-duration: 1.15s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            $("#" + response.result + "-PinnedIcon").slideDown(350);
            $("#" + response.result + "-PinOrUnpinTheDiscussion").html(' <i class="fa-solid fa-link-slash"></i> Unpin');
            $("#" + response.result + "-PinOrUnpinTheDiscussion").addClass("unpin-the-discussion");
            $("#" + response.result + "-PinOrUnpinTheDiscussion").removeClass("pin-the-discussion");
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning fa-shake" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});
$("#UnpinTheDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            alert('<i class="fa-solid fa-link-slash"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            $("#" + response.result + "-PinOrUnpinTheDiscussion").html(' <i class="fa-solid fa-thumbtack"></i> Pin');
            $("#" + response.result + "-PinnedIcon").slideUp(350);
            $("#" + response.result + "-PinOrUnpinTheDiscussion").addClass("pin-the-discussion");
            $("#" + response.result + "-PinOrUnpinTheDiscussion").removeClass("unpin-the-discussion");
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning fa-shake" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});

//DiscussionMessages//
$(document).on("submit", "#SendDiscussionMessage_Form", function (event) {
    event.preventDefault();
    let finalCut = replaceAllUsersInText($("#SendMessage_Text_Val").val());
    if (finalCut != null) $("#SendMessage_Text_Val").val(finalCut);
    finalCut = deleteAllEndEmptyChars(finalCut);

    let formData = new FormData();
    let files = $("#SendMessage_Images_Val").get(0).files;
    formData.append("discussionid", $("#SM_DiscussionId_Val").val());
    formData.append("userid", $("#SM_UserId_Val").val());
    formData.append("isautodeletable", $("#SendMessage_IsAutoDeletable").val());
    formData.append("text", finalCut);
    for (let i = 0; i < files.length; i++) {
        formData.append("images", files[i]);
    }

    $.ajax({
        url: $(this).attr("action"),
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                let sentMessagesCount = $("#SentMessagesCount_Val").val();

                $("#SendMessage_Text_Val").val(null);
                $("#SendMessage_SbmtBtn").attr("disabled", true);
                if (sentMessagesCount <= 0) $("#Presentation_Box").fadeOut(300);

                let messageMainBox = $("<div class='message-box'></div>");
                let messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
                let styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
                let isAutodeletableSmall = $("<small class='card-text'></small>");
                let mainText = $("<p class='card-text white-space-on message-label discussion-options'></p>");
                let statsBox = $("<div class='discussion-options float-end me-1'></div>");
                let isChecked = $("<small class='card-text text-primary'></small>");
                let dateAndTime = $("<small class='card-text text-muted'></small>");
                let isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");
                let imgsLinkBtn = $("<button type='button' class='btn btn-link btn-sm'></button>");
                let imgBox = $("<div class='box-container mt-1 mb-1'></div>");
                let imgTag = $("<img class='msg-img-container' alt='Cannot display this image' />");

                messageMainBox.attr("id", response.trueId + "-DiscussionMsgBox");
                messageNotMainBox.attr("id", response.trueId + "-DiscussionMsgNmBox");
                styleBox.attr("id", response.trueId + "-DiscussionMsgStyledBox");
                isEdited.attr("id", response.trueId + "-DiscussionMessageIsEdited_Lbl");

                mainText.attr("id", response.trueId + "-DiscussionOptionMsgText_Lbl");
                statsBox.attr("id", response.trueId + "-DiscussionMsgStatsBox");
                dateAndTime.attr("id", response.trueId + "-DiscussionMsgDateNTimeInfo_Lbl");

                if (response.result.text != null) {
                    replaceAllUsersInText(response.result.text);
                    let decodedText = textDecoder(response.result.text, null);
                    mainText.html(decodedText);
                }
                else {
                    mainText.text("");
                }

                if (parseInt(response.result.isAutoDeletable) > 0) {
                    isAutodeletableSmall.html(' <i class="fa-solid fa-clock-rotate-left"></i> ' + dateAndTimeTranslatorFromMins(response.result.isAutoDeletable));
                    styleBox.append(isAutodeletableSmall);
                }
                if (response.imgUrl != null) {
                    let imgSeparator = $("<div></div>");
                    imgTag.attr("src", "/DiscussionMessageImages/" + response.imgUrl);
                    imgBox.append(imgTag);
                    imgsLinkBtn.html(response.imgsCount + " Images in Stack <i class='fa-solid fa-angle-right'></i> ");
                    imgsLinkBtn.attr("id", response.trueId + "-GetDiscussionInitialMsg_Btn");
                    imgBox.attr("id", response.trueId + "-MsgImgBox");
                    imgTag.attr("id", response.trueId + "-DiscussionMsg_Img");

                    styleBox.append(imgSeparator);
                    styleBox.append(imgsLinkBtn);
                    styleBox.append(imgBox);
                }
                let date = new Date(response.result.sentAt);
                let hrs = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                let mins = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                isChecked.html(' <i class="fa-solid fa-check text-muted"></i> ');
                dateAndTime.text(hrs + ":" + mins);

                styleBox.append(mainText);
                statsBox.append(isChecked);
                statsBox.append(dateAndTime);
                statsBox.append(isEdited);
                messageNotMainBox.append(styleBox);
                messageNotMainBox.append(statsBox);
                messageMainBox.append(messageNotMainBox);

                if (sentMessagesCount <= 0) {
                    setTimeout(function () {
                        $("#DiscussionMessages_MainBox").append(messageMainBox);
                    }, 300);
                }
                else {
                    $("#DiscussionMessages_MainBox").append(messageMainBox);
                }
                $("#SentMessagesCount_Val").val(++sentMessagesCount);
                $("#StatusBar_Lbl-1").text(sentMessagesCount + " messages");
                setTimeout(function () {
                    $("#SendMessage_SbmtBtn").attr("disabled", false);
                }, 800);

                $("#PreviewTheMessage_Btn").fadeOut(250);
                $("#SendMessage_Images_Val").val(null);
                $("#EditingOrReplying_Box").slideUp(250);
                slideToTheBottom("Preloaded_Container");

                animatedClose(false, "MessagePreview_Container", true, true);
                insideBoxClose(false, "MainTextEditor_Box");
                let preloadedDiv_Bottom = parseInt($("#Preloaded_Container").css("bottom"));
                if (preloadedDiv_Bottom < 0) animatedOpen(false, "Preloaded_Container", true, true);
            }
            else {
                alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
            }

            $(".set-autodelete-duration").addClass("bg-white");
            $(".set-autodelete-duration").removeClass("bg-primary text-light");
        }
    });
});
$(document).on("submit", "#SendDiscussionReply_Form", function (event) {
    event.preventDefault();
    let files = $("#SendMessage_Images_Val").get(0).files;
    let filesMaxCount = files.length > 6 ? 6 : files.length;
    let text = deleteAllEndEmptyChars($("#SendMessage_Text_Val").val());

    let formData = new FormData();
    formData.append("discussionid", $("#SM_DiscussionId_Val").val());
    formData.append("messageid", $("#RM_ReplyId_Val").val());
    formData.append("replytext", $("#RM_ReplyText_Val").val());
    formData.append("userid", $("#SM_UserId_Val").val());
    formData.append("isautodeletable", $("#SendMessage_IsAutoDeletable").val());
    formData.append("text", text);
    for (let i = 0; i < filesMaxCount; i++) {
        formData.append("images", files[i]);
    }

    $.ajax({
        type: "POST",
        data: formData,
        url: $(this).attr("action"),
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                let sentMessagesCount = $("#SentMessagesCount_Val").val();

                $("#SendMessage_Text_Val").val(null);
                $("#SendMessage_SbmtBtn").attr("disabled", true);

                let messageMainBox = $("<div class='message-box'></div>");
                let messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
                let styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
                let isReplyBox = $("<div class='cur-user-reply-container mb-1 discussion-options reply-element'></div>");
                let repliedTo_Icon = $("<small class='card-text fw-500 discussion-options reply-element'> <i class='fa-solid fa-reply'></i> Replied to </small>");
                let replyText = $("<p class='card-text white-space-on discussion-options reply-element'></p>");
                let isAutodeletableSmall = $("<small class='card-text'></small>");
                let mainText = $("<p class='card-text white-space-on message-label discussion-options'></p>");
                let statsBox = $("<div class='discussion-options float-end me-1'></div>");
                let isChecked = $("<small class='card-text text-primary'></small>");
                let dateAndTime = $("<small class='card-text text-muted'></small>");
                let isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");
                let imgsLinkBtn = $("<button type='button' class='btn btn-link btn-sm btn-get-message-image'></button>");
                let imgBox = $("<div class='box-container mt-1 mb-1'></div>");
                let imgTag = $("<img class='msg-img-container' alt='Cannot display this image' />");

                messageMainBox.attr("id", response.trueId + "-DiscussionMsgBox");
                messageNotMainBox.attr("id", response.trueId + "-DiscussionMsgNmBox");
                styleBox.attr("id", response.trueId + "-DiscussionMsgStyledBox");
                isEdited.attr("id", response.trueId + "-DiscussionMessageIsEdited_Lbl");

                isReplyBox.attr("id", response.trueId + "-DiscussionMsgReplyBox");
                repliedTo_Icon.attr("id", response.trueId + "-DiscussionMsgReplyIcon");
                replyText.attr("id", response.trueId + "-DiscussionMsgReplyText");
                isReplyBox.attr("data-bs-msg-id", response.result.messageId);
                repliedTo_Icon.attr("data-bs-msg-id", response.result.messageId);
                replyText.attr("data-bs-msg-id", response.result.messageId);

                isReplyBox.append(repliedTo_Icon);
                isReplyBox.append(replyText);
                styleBox.append(isReplyBox);

                mainText.attr("id", response.trueId + "-DiscussionOptionMsgText_Lbl");
                statsBox.attr("id", response.trueId + "-DiscussionMsgStatsBox");
                dateAndTime.attr("id", response.trueId + "-DiscussionMsgDateNTimeInfo_Lbl");

                if (response.result.text != null) {
                    replaceAllUsersInText(response.result.text);
                    let decodedText = textDecoder(response.result.text, null);
                    mainText.html(decodedText);
                }
                else {
                    mainText.html("");
                }
                if (response.result.replyText != null) {
                    replaceAllUsersInText(response.result.replyText);
                    textDecoder(response.result.replyText, response.trueId + "-DiscussionMsgReplyText");
                }

                if (parseInt(response.result.isAutoDeletable) > 0) {
                    isAutodeletableSmall.html(' <i class="fa-solid fa-clock-rotate-left"></i> ' + response.result.isAutoDeletable + " mins");
                    styleBox.append(isAutodeletableSmall);
                }
                if (response.imgUrl != null) {
                    let imgSeparator = $("<div></div>");
                    imgTag.attr("src", "/DiscussionMessageImages/" + response.imgUrl);
                    imgBox.append(imgTag);
                    imgsLinkBtn.html(response.imgsCount + " Images in Stack <i class='fa-solid fa-angle-right'></i> ");
                    imgsLinkBtn.attr("id", response.trueId + "-GetDiscussionInitialMsg_Btn");
                    imgBox.attr("id", response.trueId + "-MsgImgBox");
                    imgTag.attr("id", response.trueId + "-DiscussionMsg_Img");

                    styleBox.append(imgSeparator);
                    styleBox.append(imgsLinkBtn);
                    styleBox.append(imgBox);
                }
                let date = new Date(response.result.sentAt);
                let hrs = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                let mins = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                isChecked.html(' <i class="fa-solid fa-check text-muted"></i> ');
                dateAndTime.text(hrs + ":" + mins);

                styleBox.append(isReplyBox);
                styleBox.append(mainText);
                statsBox.append(isChecked);
                statsBox.append(dateAndTime);
                statsBox.append(isEdited);
                messageNotMainBox.append(styleBox);
                messageNotMainBox.append(statsBox);
                messageMainBox.append(messageNotMainBox);

                if (sentMessagesCount <= 0) {
                    setTimeout(function () {
                        $("#DiscussionMessages_MainBox").append(messageMainBox);
                    }, 300);
                }
                else {
                    $("#DiscussionMessages_MainBox").append(messageMainBox);
                }
                $("#SentMessagesCount_Val").val(++sentMessagesCount);
                $("#StatusBar_Lbl-1").text(sentMessagesCount + " messages");

                $("#EditingOrReplying_Box").slideUp(250);
                $("#RM_ReplyId_Val").val(0);
                $("#RM_ReplyText_Val").val("");
                $("#SendMessage_Images_Val").val(null);
                $("#SendMessage_Text_Val").val("");
                $("#SendMessage_SbmtBtn").attr("disabled", true);
                slideToTheBottom("Preloaded_Container");
                $("#PreviewTheMessage_Btn").fadeOut(250);

                let preloadedDiv_Bottom = parseInt($("#Preloaded_Container").css("bottom"));
                if (preloadedDiv_Bottom < 0) animatedOpen(false, "Preloaded_Container", true, true);
                setTimeout(function () {
                    $("#SendMessage_SbmtBtn").attr("disabled", false);
                }, 500);
            }
            else alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
        }
    });
});
$(document).on("submit", "#EditMessage_Form", function (event) {
    let messageWithUsers = replaceAllUsersInText($("#SendMessage_Text_Val").val());
    messageWithUsers = deleteAllEndEmptyChars(messageWithUsers);
    $("#SendMessage_Text_Val").val(messageWithUsers);

    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#" + response.id + "-DiscussionMessageIsEdited_Lbl").fadeIn(350);
            $("#EditingOrReplying_Box").slideUp(350);
            $("#SendMessage_SbmtBtn").attr("disabled", true);
            $("#EM_Id_Val").val(0);
            $("#SendMessage_Text_Val").val("");
            if (response.text.length >= 40) textDecoder(response.text.substring(0, 40) + "...", response.id + "-DiscussionMsgReplyText");
            else textDecoder(response.text, response.id + "-DiscussionMsgReplyText");
            textDecoder(response.text, response.id + "-DiscussionOptionMsgText_Lbl");
            insideBoxClose(true, null);
            slideToTheBottom("Preloaded_Container");
            setTimeout(function () {
                $("#SendMessage_SbmtBtn").attr("disabled", false);
            }, 1500);
        }
        else {
            alert('<i class="fa-solid fa-ban text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

$("#PreviewTheMessage_Btn").on("click", function () {
    $("#MessagePreview_Box").empty();

    let text = textDecoder($("#SendMessage_Text_Val").val(), null);
    let isAutodeletable = $("#SendMessage_IsAutoDeletable").val();
    let isReplied = $("#RM_ReplyId_Val").val();
    let files = $("#SendMessage_Images_Val").get(0).files;

    let messageMainBox = $("<div class='message-box'></div>");
    let messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
    let styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
    let isAutodeletableSmall = $("<small class='card-text'></small>");
    let mainText = $("<p class='card-text white-space-on message-label discussion-options'></p>");
    let statsBox = $("<div class='discussion-options float-end me-1'></div>");
    let isChecked = $("<small class='card-text text-primary'> <i class='fa-solid fa-check text-muted'></i> </small>");
    let dateAndTime = $("<small class='card-text text-muted'></small>");

    mainText.html(text);
    dateAndTime.text("waiting to be sent...");
    if (parseInt(isAutodeletable) > 0) {
        isAutodeletableSmall.html(' <i class="fa-solid fa-clock-rotate-left"></i> ' + dateAndTimeTranslatorFromMins(isAutodeletable));
        styleBox.append(isAutodeletableSmall);
    }
    if (files.length > 0) {
        let filesLength = files.length;
        filesLength = filesLength > 6 ? 6 : filesLength;

        let imgSeparatorZero = $("<div></div>");
        let imgsLinkBtn = $("<button type='button' class='btn btn-link btn-sm'></button>");
        let imgBox = $("<div class='box-container bg-light mb-1 mt-1 p-2 pt-4 pb-4 text-primary text-center'></div>");

        imgsLinkBtn.html(filesLength + " Images in Stack <i class='fa-solid fa-angle-right'></i>");
        if (filesLength == 1) imgBox.html("<small class='card-text fw-500'><h3 class='h3 text-primary'> <i class='fa-regular fa-images'></i> </h3> one image would be shown here");
        else imgBox.html("<small class='card-text fw-500'><h3 class='h3 text-primary'> <i class='fa-regular fa-images'></i> </h3>" + filesLength + " images would be shown here");
        styleBox.append(imgSeparatorZero);
        styleBox.append(imgsLinkBtn);
        styleBox.append(imgBox);
    }
    
    if (parseInt(isReplied) > 0) {
        let replyText = textDecoder($("#RM_ReplyText_Val").val(), null);
        let isReplyBox = $("<div class='cur-user-reply-container mb-1 discussion-options reply-element'></div>");
        let repliedTo_Icon = $("<small class='card-text fw-500 discussion-options reply-element'> <i class='fa-solid fa-reply'></i> Replied to </small>");
        let replyText_Lbl = $("<p class='card-text white-space-on discussion-options reply-element'></p>");

        replyText_Lbl.html(replyText);
        isReplyBox.append(repliedTo_Icon);
        isReplyBox.append(replyText_Lbl);

        styleBox.append(isReplyBox);
    }
    styleBox.append(mainText);
    statsBox.append(isChecked);
    statsBox.append(dateAndTime);
    messageNotMainBox.append(styleBox);
    messageNotMainBox.append(statsBox);
    messageMainBox.append(messageNotMainBox);

    $("#EditingOrReplying_Box").slideUp(250);
    $("#MessagePreview_Box").append(messageMainBox);

    if (text.length > 0 && text.length <= 3400) $("#SendPreviewedMessage_Btn").attr("disabled", false);
    else $("#SendPreviewedMessage_Btn").attr("disabled", true);

    animatedOpen(false, "MessagePreview_Container", true, true);
});

$("#GetDiscussionsOlderMessages_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#GOM_SkipCount_Val").val(response.skipCount);
            $.each(response.result, function (index) {
                let messageMainBox;
                let messageNotMainBox;
                let styleBox;
                let isAutodeletableSmall;
                let mainText;
                let statsBox;
                let isChecked;
                let dateAndTime;
                let isEdited;
                let imgsLinkBtn;
                let imgBox;
                let imgTag;
                let isReplyBox;
                let repliedTo_Icon;
                let replyText;

                if (response.currentUserId == response.result[index].userId) {
                    messageMainBox = $("<div class='message-box'></div>");
                    messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
                    styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
                    isAutodeletableSmall = $("<small class='card-text'></small>");
                    mainText = $("<p class='card-text white-space-on message-label discussion-options'></p>");
                    statsBox = $("<div class='discussion-options float-end me-1'></div>");
                    isChecked = $("<small class='card-text text-primary'></small>");
                    dateAndTime = $("<small class='card-text text-muted'></small>");
                    isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");
                    imgsLinkBtn = $("<button type='button' class='btn btn-link btn-get-message-image btn-sm'></button>");
                    imgBox = $("<div class='box-container mt-1 mb-1'></div>");
                    imgTag = $("<img class='msg-img-container' alt='Cannot display this image' />");

                    if (response.result[index].repliedMessageId > 0) {
                        isReplyBox = $("<div class='cur-user-reply-container mb-1 reply-element'></div>");
                        repliedTo_Icon = $("<small class='card-text fw-500 reply-element'> <i class='fa-solid fa-reply'></i> Replied to</small>");
                        replyText = $("<p class='card-text white-space-on discussion-options reply-element'></p>");
                        replaceAllUsersInText(response.result[index].repliesMsgShortText);
                        let replyDecodedText = textDecoder(response.result[index].repliesMsgShortText, null);
                        replyText.html(replyDecodedText);

                        isReplyBox.attr("id", response.result[index].id + "-DiscussionMsgReplyBox");
                        repliedTo_Icon.attr("id", response.result[index].id + "-DiscussionMsgReplyIcon");
                        replyText.attr("id", response.result[index].id + "-DiscussionMsgReplyText");
                        isReplyBox.attr("data-bs-msg-id", response.result[index].id);
                        repliedTo_Icon.attr("data-bs-msg-id", response.result[index].id);
                        replyText.attr("data-bs-msg-id", response.result[index].id);
                        isReplyBox.append(repliedTo_Icon);
                        isReplyBox.append(replyText);
                    }
                }
                else {
                    messageMainBox = $("<div class='message-box'></div>");
                    messageNotMainBox = $("<div class='other-user-msg-box'></div>");
                    styleBox = $("<div class='other-user-styled-msg-box p-2'></div>");
                    isAutodeletableSmall = $("<small class='card-text'></small>");
                    mainText = $("<p class='card-text white-space-on message-label reply-to-discussion-message'></p>");
                    statsBox = $("<div class='ms-1'></div>");
                    isChecked = $("<small class='card-text text-primary'></small>");
                    dateAndTime = $("<small class='card-text text-muted'></small>");
                    isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");
                    imgsLinkBtn = $("<button type='button' class='btn btn-link-opp btn-get-message-image btn-sm'></button>");
                    imgBox = $("<div class='box-container mt-1 mb-1'></div>");
                    imgTag = $("<img class='msg-img-container' alt='Cannot display this image' />");

                    if (response.result[index].repliedMessageId > 0) {
                        isReplyBox = $("<div class='other-user-reply-container mb-1 reply-to-discussion-message reply-element'></div>");
                        repliedTo_Icon = $("<small class='card-text fw-500 reply-to-discussion-message reply-element'> <i class='fa-solid fa-reply'></i> Replied to</small>");
                        replyText = $("<p class='card-text reply-to-discussion-message message-label white-space-on reply-element'></p>");
                        replaceAllUsersInText(response.result[index].repliesMsgShortText);
                        let replyDecodedText = textDecoder(response.result[index].repliesMsgShortText, null);
                        replyText.html(replyDecodedText);

                        isReplyBox.attr("id", response.result[index].id + "-DiscussionMsgReplyBox");
                        repliedTo_Icon.attr("id", response.result[index].id + "-DiscussionMsgReplyIcon");
                        replyText.attr("id", response.result[index].id + "-DiscussionMsgReplyText");
                        isReplyBox.attr("data-bs-msg-id", response.result[index].id);
                        repliedTo_Icon.attr("data-bs-msg-id", response.result[index].id);
                        replyText.attr("data-bs-msg-id", response.result[index].id);

                        isReplyBox.append(repliedTo_Icon);
                        isReplyBox.append(replyText);
                    }
                }

                messageMainBox.attr("id", response.result[index].id + "-DiscussionMsgBox");
                messageNotMainBox.attr("id", response.result[index].id + "-DiscussionMsgNmBox");
                styleBox.attr("id", response.result[index].id + "-DiscussionMsgStyledBox");
                isEdited.attr("id", response.result[index].id + "-DiscussionMessageIsEdited_Lbl");

                mainText.attr("id", response.result[index].id + "-DiscussionOptionMsgText_Lbl");
                statsBox.attr("id", response.result[index].id + "-DiscussionMsgStatsBox");
                dateAndTime.attr("id", response.result[index].id + "-DiscussionMsgDateNTimeInfo_Lbl");

                let date = new Date(response.result[index].sentAt);
                let hrs = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                let mins = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();

                replaceAllUsersInText(response.result[index].text);
                let decodedText = textDecoder(response.result[index].text, null);
                mainText.html(decodedText);
                dateAndTime.text(hrs + ":" + mins);

                if (response.result[index].isChecked) {
                    isChecked.html(' <i class="fa-solid fa-check-double text-primary"></i> ');
                }
                else {
                    isChecked.html(' <i class="fa-solid fa-check text-muted"></i> ');
                }
                if (response.result[index].isEdited) {
                    isEdited.text(" edited");
                    isEdited.fadeIn(100);
                }
                if (parseInt(response.result[index].isAutoDeletable) > 0) {
                    isAutodeletableSmall.html(' <i class="fa-solid fa-clock-rotate-left"></i> ' + dateAndTimeTranslatorFromMins(response.result[index].isAutoDeletable));
                    styleBox.append(isAutodeletableSmall);
                }
                if (response.result[index].imgUrl != null) {
                    let imgSeparator = $("<div></div>");
                    imgTag.attr("src", "/DiscussionMessageImages/" + response.result[index].imgUrl);
                    imgBox.append(imgTag);
                    imgsLinkBtn.html("More Images in Stack <i class='fa-solid fa-angle-right'></i> ");
                    imgBox.attr("id", response.result[index].id + "-MsgImgBox");
                    imgsLinkBtn.attr("id", response.result[index].id + "-GetDiscussionInitialMsg_Btn");
                    imgTag.attr("id", response.result[index].id + "-DiscussionMsg_Img");

                    styleBox.append(imgSeparator);
                    styleBox.append(imgsLinkBtn);
                    styleBox.append(imgBox);
                }
                if (response.result[index].repliedMessageId > 0) {
                    styleBox.append(isReplyBox);
                }
                styleBox.append(mainText);
                statsBox.append(isChecked);
                statsBox.append(dateAndTime);
                statsBox.append(isEdited);
                messageNotMainBox.append(styleBox);
                messageNotMainBox.append(statsBox);
                messageMainBox.append(messageNotMainBox);

                textDecoder(response.result[index].text, response.result[index].id + "-DiscussionOptionMsgText_Lbl");
                $("#DiscussionMessages_MainBox").prepend(messageMainBox);
            });
        }
    });
});

$("#GetPinnedMessageInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let skipCount = parseInt($("#GPMI_SkipCount_Val").val());
            let currentPinnedMessagesNumber = parseInt($("#PinnedMessagesCurrentNumber_Span").text());
            let maxPinnedMessagesCount = parseInt($("#PinnedMessagesCount_Span").text());
            let nextPinnedMessageNumber = currentPinnedMessagesNumber >= maxPinnedMessagesCount ? 1 : ++currentPinnedMessagesNumber;
            textDecoder(response.result.text, "CurrentPinnedMessagesText_Lbl");
            $("#CurrentPinnedMessagesText_Lbl").attr("data-bs-html", response.result.id);

            if (nextPinnedMessageNumber >= maxPinnedMessagesCount) {
                $("#GPMI_SkipCount_Val").val(0);
                $("#PinnedMessagesCurrentNumber_Span").text(nextPinnedMessageNumber);
            }
            else {
                $("#GPMI_SkipCount_Val").val(++skipCount);
                $("#PinnedMessagesCurrentNumber_Span").text(nextPinnedMessageNumber);
            }
        }
        else alert('<i class="fa-regular fa-times-circle fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
    });
});
$("#PinTheDiscussionMessage_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            insideBoxClose(true, null);
            let currentPinnedMessagesCount = parseInt($("#PinnedMessagesCount_Span").text()) + 1;
            let messageText = $("#" + response.id + "-DiscussionOptionMsgText_Lbl").html();
            $("#PinnedMessagesCurrentNumber_Span").text("1");
            $("#PinnedMessagesCount_Span").text(currentPinnedMessagesCount);
            $("#GPMI_SkipCount_Val").val(1);
            textDecoder(messageText, "CurrentPinnedMessagesText_Lbl");
            $("#CurrentPinnedMessagesText_Lbl").attr("data-bs-html", response.id);

            if (currentPinnedMessagesCount <= 1) slideToRight("Preloaded_Container_PostHeader");
            alert('<i class="fa-solid fa-thumbtack fa-bounce text-primary" style="--fa-animation-duration: 1.2s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", "Copy", 0, 2, null, "CurrentPinnedMessagesText_Lbl", 3.75);
        }
        else alert('<i class="fa-regular fa-times-circle fa-shake" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
    });
});
$("#UnpinTheDiscussionMessage_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            insideBoxClose(true, null);
            let currentPinnedMessagesCount = parseInt($("#PinnedMessagesCount_Span").text()) - 1;
            if (response.id > 0) {
                let messageText = $("#" + response.id + "-DiscussionOptionMsgText_Lbl").html();
                textDecoder(messageText, "CurrentPinnedMessagesText_Lbl");
            }
            else $("#CurrentPinnedMessagesText_Lbl").text("No pinned messages");
            $("#PinnedMessagesCurrentNumber_Span").text("1");
            $("#PinnedMessagesCount_Span").text(currentPinnedMessagesCount);
            $("#GPMI_SkipCount_Val").val(1);
            $("#CurrentPinnedMessagesText_Lbl").attr("data-bs-html", response.id);

            if (currentPinnedMessagesCount <= 0) slideToLeft("Preloaded_Container_PostHeader");
            alert('<i class="fa-solid fa-thumbtack fa-bounce text-primary" style="--fa-animation-duration: 1.2s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", "Copy", 0, 2, null, "CurrentPinnedMessagesText_Lbl", 3.75);
        }
        else alert('<i class="fa-solid fa-link-slash"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
    });
});

$("#GetPrevDiscussionMessageImg_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let setableSkipCount = parseInt(response.skipCount);
            let showableSkipCount = parseInt(++response.skipCount);

            if ($("#" + setableSkipCount + "-ImagesShow_Val").val() == undefined) {
                let hdnInput = $("<input type='hidden' />");
                hdnInput.attr("id", setableSkipCount + "-ImagesShow_Val");
                hdnInput.val(response.result.url);
                $("#ImagesShowVals_Box").append(hdnInput);
            }

            $("#ImagesCounter_Lbl").text(showableSkipCount);
            $("#ImagesFullCount_Lbl").text(response.fullCount);
            $("#GNDMI_SkipCount_Val").val(setableSkipCount);
            $("#GPDMI_SkipCount_Val").val(setableSkipCount);
            $("#GPDMI_FullCount_Val").val(response.fullCount);
            $("#GNDMI_FullCount_Val").val(response.fullCount);
            $("#ImageShow_Img").attr("src", "/DiscussionMessageImages/" + response.result.url);
            $("#ImagesShowImg_Lbl").text(response.result.url);

            if (showableSkipCount >= parseInt(response.fullCount)) {
                $("#GPDMSbmt_Btn").attr("disabled", false);
                $("#GNDMSbmt_Btn").attr("disabled", true);
            }
            else if (showableSkipCount <= 1) {
                $("#GPDMSbmt_Btn").attr("disabled", true);
                $("#GNDMSbmt_Btn").attr("disabled", false);
            }
            else {
                $("#GPDMSbmt_Btn").attr("disabled", false);
                $("#GNDMSbmt_Btn").attr("disabled", false);
            }

            if (parseInt($("#ImagesShow_Container").css("bottom")) < 0) animatedOpen(false, "ImagesShow_Container", true, true);
        }
        else alert('<i class="fa-regular fa-eye-slash"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
    });
});
$("#GetNextDiscussionMessageImg_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let setableSkipCount = parseInt(response.skipCount);
            let showableSkipCount = parseInt(++response.skipCount);

            if (response.startTry) {
                showableSkipCount = 1;
                setableSkipCount = 0;
            }
            if ($("#" + setableSkipCount + "-ImagesShow_Val").val() == undefined) {
                let hdnInput = $("<input type='hidden' />");
                hdnInput.attr("id", setableSkipCount + "-ImagesShow_Val");
                hdnInput.val(response.result.url);
                $("#ImagesShowVals_Box").append(hdnInput);
            }

            $("#ImagesCounter_Lbl").text(showableSkipCount);
            $("#ImagesFullCount_Lbl").text(response.fullCount);
            $("#GNDMI_SkipCount_Val").val(setableSkipCount);
            $("#GPDMI_SkipCount_Val").val(setableSkipCount);
            $("#GPDMI_FullCount_Val").val(response.fullCount);
            $("#GNDMI_FullCount_Val").val(response.fullCount);
            $("#ImageShow_Img").attr("src", "/DiscussionMessageImages/" + response.result.url);
            $("#ImagesShowImg_Lbl").text(response.result.url);

            if (showableSkipCount >= parseInt(response.fullCount)) {
                $("#GPDMSbmt_Btn").attr("disabled", false);
                $("#GNDMSbmt_Btn").attr("disabled", true);
            }
            else if (showableSkipCount <= 1) {
                $("#GPDMSbmt_Btn").attr("disabled", true);
                $("#GNDMSbmt_Btn").attr("disabled", false);
            }
            else {
                $("#GPDMSbmt_Btn").attr("disabled", false);
                $("#GNDMSbmt_Btn").attr("disabled", false);
            }

            if (parseInt($("#ImagesShow_Container").css("bottom")) < 0) animatedOpen(false, "ImagesShow_Container", true, true);
        }
        else alert('<i class="fa-regular fa-eye-slash"></i>', response.alert, "Got It", null, 0, null, null, null, 3.25);
    });
});

$("#GPDMSbmt_Btn").on("click", function () {
    let skipCount = parseInt($("#GPDMI_SkipCount_Val").val()) - 1;
    skipCount = skipCount < 0 ? 0 : skipCount;
    if ($("#" + skipCount + "-ImagesShow_Val").val() == undefined) {
        $("#GetPrevDiscussionMessageImg_Form").submit();
    }
    else {
        let fullCount = parseInt($("#GNDMI_FullCount_Val").val());
        $("#GPDMI_SkipCount_Val").val(skipCount);
        $("#GNDMI_SkipCount_Val").val(skipCount);
        $("#ImageShow_Img").attr("src", "/DiscussionMessageImages/" + $("#" + skipCount + "-ImagesShow_Val").val());
        $("#ImagesShowImg_Lbl").text($("#" + skipCount + "-ImagesShow_Val").val());
        $("#ImagesCounter_Lbl").text(++skipCount);

        if (skipCount >= fullCount) {
            $("#GPDMSbmt_Btn").attr("disabled", false);
            $("#GNDMSbmt_Btn").attr("disabled", true);
        }
        else if (skipCount <= 1) {
            $("#GPDMSbmt_Btn").attr("disabled", true);
            $("#GNDMSbmt_Btn").attr("disabled", false);
        }
        else {
            $("#GPDMSbmt_Btn").attr("disabled", false);
            $("#GNDMSbmt_Btn").attr("disabled", false);
        }
    }
});
$("#GNDMSbmt_Btn").on("click", function () {
    let skipCount = parseInt($("#GNDMI_SkipCount_Val").val()) + 1;
    let fullCount = parseInt($("#GNDMI_FullCount_Val").val());
    skipCount = skipCount >= fullCount ? 0 : skipCount;
    $("#GPDMI_StartTry_Val").val(false);

    if ($("#" + skipCount + "-ImagesShow_Val").val() == undefined) {
        $("#GetNextDiscussionMessageImg_Form").submit();
    }
    else { 
        $("#GPDMI_SkipCount_Val").val(skipCount);
        $("#GNDMI_SkipCount_Val").val(skipCount);
        $("#ImageShow_Img").attr("src", "/DiscussionMessageImages/" + $("#" + skipCount + "-ImagesShow_Val").val());
        $("#ImagesShowImg_Lbl").text($("#" + skipCount + "-ImagesShow_Val").val());
        $("#ImagesCounter_Lbl").text(++skipCount);
        if (skipCount >= fullCount) {
            $("#GPDMSbmt_Btn").attr("disabled", false);
            $("#GNDMSbmt_Btn").attr("disabled", true);
        }
        else if (skipCount <= 1) {
            $("#GPDMSbmt_Btn").attr("disabled", true);
            $("#GNDMSbmt_Btn").attr("disabled", false);
        }
        else {
            $("#GPDMSbmt_Btn").attr("disabled", false);
            $("#GNDMSbmt_Btn").attr("disabled", false);
        }
    }
});
$(document).on("click", ".btn-get-message-image", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#ImagesShowVals_Box").empty();
        $("#GPDMI_Id_Val").val(trueId);
        $("#GPDMI_SkipCount_Val").val(0);
        $("#GPDMI_FullCount_Val").val(0);
        $("#GNDMI_Id_Val").val(trueId);
        $("#GNDMI_SkipCount_Val").val(0);
        $("#GNDMI_FullCount_Val").val(0);
        $("#GPDMI_StartTry_Val").val(true);
        $("#GetNextDiscussionMessageImg_Form").submit();
    }
    else {
        $("#ImagesShowVals_Box").empty();
        $("#GPDMI_Id_Val").val(0);
        $("#GPDMI_SkipCount_Val").val(0);
        $("#GPDMI_FullCount_Val").val(0);
        $("#GNDMI_Id_Val").val(0);
        $("#GNDMI_SkipCount_Val").val(0);
        $("#GNDMI_FullCount_Val").val(0);
        $("#GNDMI_FullCount_Val").val(true);
    }
});

//Discussion Messages//
$("#GetMessageInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            let isChecked;
            let isEdited;
            if (response.result.isEdited) {
                isEdited = ", edited";
            }
            else isEdited = "";
            if (response.result.isChecked) isChecked = ' <i class="fa-solid fa-check-double text-primary"></i> ';
            else isChecked = ' <i class="fa-solid fa-check"></i> ';
            //GetDiscussions_Form
            $("#EditingOrReplying_Box").slideUp(300);

            if (response.result.isPinned) {
                $("#UDM_Id_Val").val(response.id);
                $("#PDM_Id_Val").val(0);
                $("#PinTheMessage_Box").fadeOut(0);
                $("#UnpinTheMessage_Box").fadeIn(0);
            }
            else {
                $("#UDM_Id_Val").val(0);
                $("#PDM_Id_Val").val(response.id);
                $("#PinTheMessage_Box").fadeIn(0);
                $("#UnpinTheMessage_Box").fadeOut(0);
            }

            if (response.userId == response.result.userId) {
                $("#EditDiscussionMsg_Col").fadeIn(600);
                $("#DeleteDiscussionMsg_Col").fadeIn(600);
                $("#DDM_Id_Val").val(response.id);

                if (response.result.daysPassed > 4) {
                    $("#EditDiscussionMessage_Btn").attr("disabled", true);
                }
                else {
                    $("#EditDiscussionMessage_Btn").attr("disabled", false);
                }
            }
            else {
                $("#EditDiscussionMsg_Col").fadeOut(0);
                $("#DeleteDiscussionMsg_Col").fadeOut(0);
            }
            $(".send-message-variety").attr("action", "/DiscussionMessage/Message");
            $(".send-message-variety").attr("id", "SendDiscussionMessage_Form");

            $("#EM_Id_Val").val(response.id);
            $("#DDM_Id_Val").val(response.id);
            $("#GDMA_Id_Val").val(response.id);
            $("#SDMA_MessageId_Val").val(response.id);
            $("#SDMR_Id_Val").val(response.id);
            $("#GDMR_Id_Val").val(response.id);
            $("#RM_ReplyId_Val").val(response.id);
            $("#RM_ReplyText_Val").val(response.result.text.substring(0, 40));
            $("#GDMA_MsgText_Val").val(response.result.text.substring(0, 40));

            setTimeout(function () {
                $("#DiscussionOptionAdditionalInfo_Lbl").fadeIn(300);
                $("#DiscussionOptionMessageText_Lbl").html(response.result.text);
                textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "DiscussionOptionMessageText_Lbl");
                textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "EditingOrReplyingMsgText_Lbl");
                $("#DiscussionOptionAdditionalInfo_Lbl").html(isChecked + " " + dateAndTimeTranslator(response.result.sentAt) + isEdited);
                $("#CurrentGotDiscussionMsg_Id_Val").val(response.id);

                insideBoxOpen("DiscussionOptions_Box");
            }, 225);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#MarkasReadDiscussionMessage_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (!response.success) {
            alert('<i class="fa-regular fa-circle-xmark fa-shake text-danger" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
        }
    });
});
$("#DeleteDiscussionMessage_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let currentCount = $("#SentMessagesCount_Val").val();
            currentCount--;
            if (currentCount <= 0) {
                $("#NoSentMessages_Box").css("margin-bottm", "-1200px");
                $("#StatusBar_Lbl-1").text("no sent messages");
                $("#" + response.id + "-DiscussionMsgBox").fadeOut(350);
                setTimeout(function () {
                    insideBoxOpen("NoSentMessages_Box");
                }, 300);
            }
            else {
                $("#StatusBar_Lbl-1").text(currentCount + " sent messages");
                $("#" + response.id + "-DiscussionMsgBox").fadeOut(350);
                $("#" + response.id + "-DiscussionMsgReplyText").text("Deleted message");
            }
            $("#DDM_Id_Val").val(0)
            insideBoxClose(false, "DiscussionOptions_Box");
            $("#SentMessagesCount_Val").val(currentCount);
        }
        else {
            alert('<i class="fa-solid fa-ban text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

//Discussion Answers//
$(document).on("submit", "#SendDiscussionMessageAnswer_Form", function (event) {
    event.preventDefault();

    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let answersCount = parseInt($("#AnswersCount_Val").val());

            let messageMainBox = $("<div class='message-box style='display: none;'></div>");
            let messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
            let styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
            let mainText = $("<p class='card-text white-space-on message-label answer-options'></p>");
            let statsBox = $("<div class='answer-options float-end me-1'></div>");
            let dateAndTime = $("<small class='card-text text-muted'></small>");
            let isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");

            messageMainBox.attr("id", response.id + "-AnswerMain_Box");
            messageNotMainBox.attr("id", response.id + "-AnswerNotMain_Box");
            styleBox.attr("id", response.id + "-AnswerStyle_Box");
            statsBox.attr("id", response.id + "-AnswerStats_Box");
            mainText.attr("id", response.id + "-AnswerMainText_Lbl");
            dateAndTime.attr("id", response.id + "-AnswerDateAndTime_Lbl");
            isEdited.attr("id", response.id + "-IsEdited_Lbl");

            mainText.html(response.result.text);
            dateAndTime.text(dateAndTimeConverter(response.result.createdAt));

            styleBox.append(mainText);
            statsBox.append(dateAndTime);
            statsBox.append(isEdited);
            messageNotMainBox.append(styleBox);
            messageNotMainBox.append(statsBox);
            messageMainBox.append(messageNotMainBox);

            if (answersCount <= 0) {
                $("#AnswersInfo_Box").empty();
                setTimeout(function () {
                    $("#AnswersInfo_Box").append(messageMainBox);
                    setTimeout(function () {
                        messageMainBox.slideDown(225);
                    }, 100);
                }, 75);
            }
            else {
                $("#AnswersInfo_Box").append(messageMainBox);
                setTimeout(function () {
                    messageMainBox.slideDown(225);
                }, 75);
            }
            answersCount++;
            $("#AnswersCount_Val").val(answersCount);
            if (answersCount == 1) $("#AnswersCount_Lbl").text("one answer");
            else $("#AnswersCount_Lbl").text(answersCount + " answers");
        }
        else alert(' <i class="fa-solid fa-comment-slash fa-shake text-warning" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i> ', response.alert, "Close", null, 0, null, null, null, 3.25);
    });
});
$(document).on("submit", "#EditDiscussionMessageAnswer_Form", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        $(".send-answer-variety").attr("action", "/Answer/SendDiscussionMessageAnswer");
        $(".send-answer-variety").attr("id", "SendDiscussionMessageAnswer_Form");
        $("#SDMA_Id_Val").val(0);
        $("#SDMA_Text_Val").val("");
        $(".cancel-answer-options").fadeOut(250);
        $("#SDMASbmt_Btn").text("Send");

        if (response.success) {
            $("#" + response.result + "-AnswerMainText_Lbl").text(response.text);
            $("#" + response.result + "-IsEdited_Lbl").text(", edited");
            $("#" + response.result + "-IsEdited_Lbl").fadeIn(300);
        }
        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
    });
}); 
$("#DeleteDiscussionMessageAnswer_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            let answersCount = parseInt($("#AnswersCount_Val").val());
            answersCount--;

            $("#AnswersCount_Val").val(answersCount);
            if (answersCount <= 0) $("#AnswersCount_Lbl").text("no answers yet");
            else if (answersCount == 1) $("#AnswersCount_Lbl").text("one answer");
            else $("#AnswersCount_Lbl").text(answersCount + " answers");

            $(".cancel-answer-options").fadeOut(300);
            $(".delete-answer-option").fadeOut(300);
            $("#ADDM_Id_Val").val(0);
            $("#SDMA_Id_Val").val(0);
            $("#SDMA_Text_Val").val("");

            slideToLeft(response.result + "-AnswerMain_Box");
            setTimeout(function () {
                $("#" + response.result + "-AnswerMain_Box").fadeOut(150);
            }, 325);

            if (answersCount <= 0) {
                let noAnswersBox = $("<div class='box-container bg-light p-2 text-center'></div>");
                let noAnswersIcon = $("<span class='h1 text-primary'> <i class='fa-regular fa-message'></i> </span>");
                let noAnswersText = $("<span class='h5'>No Answers</span>");
                let noAnswersSeparatorZero = $("<div class='mt-1'></div>");
                let noAnswersSeparatorOne = $("<div></div>");
                let noAnswersDescription = $("<small class='card-text text-muted'>this message hasn't got any answer yet</small>");

                noAnswersBox.append(noAnswersIcon);
                noAnswersBox.append(noAnswersSeparatorZero);
                noAnswersBox.append(noAnswersText);
                noAnswersBox.append(noAnswersSeparatorOne);
                noAnswersBox.append(noAnswersDescription);
                setTimeout(function () {
                    $("#AnswersInfo_Box").append(noAnswersBox);
                }, 485);
            }
        }
        else alert('<i class="fa-regular fa-trash-can fa-shake text-danger" style="--fa-animation-duration: 1.2s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
    });
});

$("#GetDiscussionMessageAnswers_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        animatedClose(false, "Answers_Container", true, true);
        $("#AnswersCount_Val").val(response.count);
        setTimeout(function () {
            $("#AnswersInfo_Box").empty();
        }, 300);

        if (response.message.length >= 40) {
            textDecoder(response.message + "...", "AnswerMsgText_Lbl");
        }
        else textDecoder(response.message, "AnswerMsgText_Lbl");
        if (parseInt(response.count) <= 0) $("#AnswersCount_Lbl").text("no answers");
        else if (parseInt(response.count) == 1) $("#AnswersCount_Lbl").text("one answer");
        else $("#AnswersCount_Lbl").text(response.count + " answers");

        if (response.success) {
            if (response.count > 0) {
                setTimeout(function () {
                    $.each(response.result, function (index) {
                        if (response.result[index].userId == response.currentUserId) {
                            let messageMainBox = $("<div class='message-box'></div>");
                            let messageNotMainBox = $("<div class='cur-user-msg-box'></div>");
                            let styleBox = $("<div class='cur-user-styled-msg-box p-2'></div>");
                            let mainText = $("<p class='card-text white-space-on answer-options'></p>");
                            let statsBox = $("<div class='discussion-options float-end me-1'></div>");
                            let dateAndTime = $("<small class='card-text text-muted'></small>");
                            let isEdited = $("<small class='card-text text-muted' style='display: none;'>, edited</small>");

                            messageMainBox.attr("id", response.result[index].id + "-AnswerMain_Box");
                            messageNotMainBox.attr("id", response.result.id + "-AnswerNotMain_Box");
                            styleBox.attr("id", response.result[index].id + "-AnswerStyle_Box");
                            statsBox.attr("id", response.result[index].id + "-AnswerStats_Box");
                            mainText.attr("id", response.result[index].id + "-AnswerMainText_Lbl");
                            dateAndTime.attr("id", response.result[index].id + "-AnswerDateAndTime_Lbl");
                            isEdited.attr("id", response.result[index].id + "-IsEdited_Lbl");

                            if (response.result[index].isEdited) isEdited.fadeIn(0);

                            mainText.html(response.result[index].text, response.result[index].id + "-AnswerMainText_Lbl");
                            dateAndTime.text(dateAndTimeTranslator(response.result[index].createdAt));

                            styleBox.append(mainText);
                            statsBox.append(dateAndTime);
                            statsBox.append(isEdited);
                            messageNotMainBox.append(styleBox);
                            messageNotMainBox.append(statsBox);
                            messageMainBox.append(messageNotMainBox);

                            $("#AnswersInfo_Box").append(messageMainBox);
                        }
                        else {
                            let messageMainBox = $("<div class='message-box'></div>");
                            let messageNotMainBox = $("<div class='other-user-msg-box'></div>");
                            let styleBox = $("<div class='other-user-styled-msg-box p-2'></div>");
                            let mainText = $("<p class='card-text white-space-on'></p>");
                            let statsBox = $("<div class='ms-1'></div>");
                            let dateAndTime = $("<small class='card-text text-muted'></small>");
                            let isEdited = $("<small class='card-text text-muted' style='display: none;'></small>");

                            messageMainBox.attr("id", response.result[index].id + "-AnswerMain_Box");
                            messageNotMainBox.attr("id", response.result.id + "-AnswerNotMain_Box");
                            styleBox.attr("id", response.result[index].id + "-AnswerStyle_Box");
                            statsBox.attr("id", response.result[index].id + "-AnswerStats_Box");
                            mainText.attr("id", response.result[index].id + "-AnswerMainText_Lbl");
                            dateAndTime.attr("id", response.result[index].id + "-AnswerDateAndTime_Lbl");
                            isEdited.attr("id", response.result[index].id + "-IsEdited_Lbl");

                            mainText.html(response.result[index].text, response.result[index].id + "-AnswerMainText_Lbl");
                            dateAndTime.text(dateAndTimeTranslator(response.result[index].createdAt));

                            styleBox.append(mainText);
                            statsBox.append(dateAndTime);
                            statsBox.append(isEdited);
                            messageNotMainBox.append(styleBox);
                            messageNotMainBox.append(statsBox);
                            messageMainBox.append(messageNotMainBox);

                            $("#AnswersInfo_Box").append(messageMainBox);
                        }
                    });
                }, 400);

                insideBoxClose(true, null);
                animatedClose(false, "DiscussionInfoTooltips_Container", false, true);
                setTimeout(function () {
                    openSidebar();
                    animatedOpen(false, "Answers_Container", true, false);
                }, 450);
            }
            else {
                let noAnswersBox = $("<div class='box-container bg-light p-2 text-center'></div>");
                let noAnswersIcon = $("<span class='h1 text-primary'> <i class='fa-regular fa-message'></i> </span>");
                let noAnswersText = $("<span class='h5'>No Answers</span>");
                let noAnswersSeparatorZero = $("<div class='mt-1'></div>");
                let noAnswersSeparatorOne = $("<div></div>");
                let noAnswersDescription = $("<small class='card-text text-muted'>this message hasn't got any answer yet</small>");

                noAnswersBox.append(noAnswersIcon);
                noAnswersBox.append(noAnswersSeparatorZero);
                noAnswersBox.append(noAnswersText);
                noAnswersBox.append(noAnswersSeparatorOne);
                noAnswersBox.append(noAnswersDescription);

                insideBoxClose(true, null);
                animatedClose(false, "DiscussionInfoTooltips_Container", false, true);
                setTimeout(function () {
                    $("#AnswersInfo_Box").append(noAnswersBox);
                    openSidebar();
                    animatedOpen(false, "Answers_Container", true, false);
                }, 450);
            }
        }
        else {
            let noAnswersBox = $("<div class='box-container bg-light p-2 text-center'></div>");
            let noAnswersIcon = $("<span class='h1 text-primary'> <i class='fa-regular fa-message'></i> </span>");
            let noAnswersText = $("<span class='h5'>No Answers</span>");
            let noAnswersSeparatorZero = $("<div class='mt-1'></div>");
            let noAnswersSeparatorOne = $("<div></div>");
            let noAnswersDescription = $("<small class='card-text text-muted'>this message hasn't got any answer yet</small>");

            noAnswersBox.append(noAnswersIcon);
            noAnswersBox.append(noAnswersSeparatorZero);
            noAnswersBox.append(noAnswersText);
            noAnswersBox.append(noAnswersSeparatorOne);
            noAnswersBox.append(noAnswersDescription);

            insideBoxClose(true, null);
            animatedClose(false, "DiscussionInfoTooltips_Container", false, true);
            setTimeout(function () {
                $("#AnswersInfo_Box").append(noAnswersBox);
                openSidebar();
                animatedOpen(false, "Answers_Container", true, false);
            }, 450);
        }
    });
});
$(document).on("click", ".answer-options", function (event) {
    let trueId = getTrueId(event.target.id);

    $("#ADDM_Id_Val").val(0);
    if (trueId != null) {
        $(".send-answer-variety").attr("action", "/Answer/EditDiscussionMessageAnswer");
        $(".send-answer-variety").attr("id", "EditDiscussionMessageAnswer_Form");
        $("#SDMA_Id_Val").val(trueId);
        $("#ADDM_Id_Val").val(trueId);
        $(".cancel-answer-options").fadeIn(300);
        $(".delete-answer-option").fadeIn(300);
        $("#SDMA_Text_Val").val($("#" + trueId + "-AnswerMainText_Lbl").text());
        $("#SDMASbmt_Btn").text("Save");
    }
    else {
        $(".send-answer-variety").attr("action", "/Answer/SendDiscussionMessageAnswer");
        $(".send-answer-variety").attr("id", "SendDiscussionMessageAnswer_Form");
        $("#SDMA_Id_Val").val(0);
        $("#ADDM_Id_Val").val(0);
        $("#SDMA_Text_Val").val("");
        $(".cancel-answer-options").fadeOut(300);
        $(".delete-answer-option").fadeOut(300);
        $("#SDMASbmt_Btn").text("Send");
    }
});
$(document).on("click", ".delete-answer-option", function () {
    $("#DeleteDiscussionMessageAnswer_Form").submit();
});
$(document).on("click", ".cancel-answer-options", function () {
    $(".cancel-answer-options").fadeOut(300);
    $(".delete-answer-option").fadeOut(300);
    $(".send-answer-variety").attr("action", "/Answer/SendDiscussionMessageAnswer");
    $(".send-answer-variety").attr("id", "SendDiscussionMessageAnswer_Form");

    $("#SDMA_Id_Val").val(0);
    $("#SDMA_Text_Val").val("");
    $("#SDMASbmt_Btn").text("Send");
    $("#SDMA_Box").slideDown(250);

    $("#ADDM_Id_Val").val(0);
});

$("#FindUsersToAdd_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();
    $.get(url, data, function (response) {
        $("#FindUsersResults_Box").empty();

        if (response.success) {
            if (response.count > 0) {
                let countLbl = $("<h5 class='h5 text-center mb-3'></h5>");
                countLbl.text(response.count.toLocaleString() + " results");
                $("#FindUsersResults_Box").append(countLbl);
                $.each(response.result, function (index) {
                    let container = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let header = $("<span class='h6'></span>");
                    let separatorZero = $("<div></div>");
                    let shortLink = $("<small class='card-text text-muted'></small>");
                    let addBtn = $("<button type='button' class='btn btn-primary btn-standard-asset btn-sm float-end ms-1 add-member-to-discussion'> <i class='fa-solid fa-plus'></i> Add</button>");

                    addBtn.attr("id", response.result[index].id + "-AddMemberToDiscussion");
                    header.html(response.result[index].pseudoName);
                    shortLink.html("@" + response.result[index].shortName);

                    container.append(addBtn);
                    container.append(header);
                    container.append(separatorZero);
                    container.append(shortLink);

                    $("#FindUsersResults_Box").append(container);
                });
            }
            else {
                let container = $("<div class='box-container text-center'></div>");
                let noResultsIcon_Lbl=$("<h1 class='h1'> <i class='fa-regular fa-circle-xmark fa-shake' style='--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;'></i> </h2>")
                let noUsersMainLbl = $("<h4 class='h4'>No Results</h4>");
                let header = $('<small class="card-text text-muted"> <i class="fa-solid fa-magnifying-glass"></i> Find new members to add them to this discussion</small>');

                container.append(noResultsIcon_Lbl);
                container.append(noUsersMainLbl);
                container.append(header);

                $("#FindUsersResults_Box").append(container);
            }
        }
        else {
            let container = $("<div class='box-container text-center'></div>");
            let header = $('<small class="card-text text-muted"> <i class="fa-solid fa-magnifying-glass"></i> </small>' + response.alert);

            container.append(header);
            $("#FindUsersResults_Box").append(container);
        }
    });
});

$("#GetDiscussionCommands_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        $("#CreatedAdditionalCommands_Box").empty();
        $("#CreateCommand_Box").fadeOut(300);
        $("#CommandsInfo_Box").fadeIn(300);
        if (response.success) {
            if (response.count > 0) {
                $.each(response.result, function (index) {
                    let div = $("<div></div>");
                    let small = $("<small class='card-text text-muted'></small>");
                    let insideSpan = $("<span class='fw-50'></span>");
                    insideSpan.text(response.result[index].command);
                    small.text(response.result[index].description);

                    small.append(span);
                    div.append(small);
                    $("#CreatedAdditionalCommands_Box").append(div);
                });
            }
            else {
                let div = $("<div></div>");
                let small = $("<small class='card-text text-muted'>No additional created commands for this discussion</small>");
                div.append(small);

                $("#CreatedAdditionalCommands_Box").append(div);
            }
        }
        else {
            alert('<i class="fa-solid fa-terminal text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
            let div = $("<div></div>");
            let small = $("<small class='card-text text-muted'>No additional created commands for this discussion</small>");
            let separatorZero = $("<div class='mt-2'></div>");
            let createCmd_Btn = $("<button type='button' class='btn btn-light btn-standard-asset shadow-none btn-sm btn-execute-cmd'> <i class='fa-solid fa-plus'></i> Create a Command</button>");
            createCmd_Btn.attr("data-bs-html", "/create");

            div.append(small);
            div.append(separatorZero);
            div.append(createCmd_Btn);

            $("#CreatedAdditionalCommands_Box").append(div);
        }
        $("#CreatedAdditionalCommands_Box").fadeIn(350);
        $("#AdditionalCommands_Box").fadeIn(350);
    });
});

$("#IsShortlinkFree_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#ShortLinkFreedomness_Lbl").html("Entered shortlink is <span class='text-primary'>free to use</span>");
            $("#DiscussionInfoEdit_SbmtBtn").attr("disabled", false);
        }
        else {
            $("#ShortLinkFreedomness_Lbl").html("Entered shortlink is <span class='text-danger'>not free to use</span>. Try to use another one");
            $("#DiscussionInfoEdit_SbmtBtn").attr("disabled", true);
        }

        setTimeout(function () {
            $("#ShortLinkFreedomness_Lbl").text("Required characters for shortlink are: [A-Z, a-z], [0-9] and [_]. Required shortlink length: from 4 and up to 20 characters");
        }, 5000);
    });
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

$("#DiscussionShortlink_Val").on("change", function () {
    $("#IDSF_Shortlink_Val").val($(this).val());
    if ($(this).val().length >= 4 && $(this).val().length <= 20) {
        $("#IsShortlinkFree_Form").submit();
    }
    else {
        $("#DiscussionInfoEdit_SbmtBtn").attr("disabled", true);
        $("#ShortLinkFreedomness_Lbl").html("Shortlink is <span class='text-danger'>too short</span>. It should contain <span class='fw-500'>[4-20] chars</span>");
        $("#IDSF_Shortlink_Val").val($("#CurrentShortLink_Val").val());
        $("#DiscussionShortlink_Val").val($("#CurrentShortLink_Val").val());

        setTimeout(function () {
            $("#ShortLinkFreedomness_Lbl").text("Required characters for shortlink are: [A-Z, a-z], [0-9] and [_]. Required shortlink length: from 4 and up to 20 characters");
        }, 6000);
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

$("#CancelDiscussionMessageSettings_Btn").on("click", function () {
    $(".send-message-variety").attr("action", "/DiscussionMessage/Message");
    $(".send-message-variety").attr("id", "SendDiscussionMessage_Form");
    $("#RM_ReplyId_Val").val(0);
    $("#RM_ReplyText_Val").val("");
    $("#EM_Id_Val").val(0);
    $("#SendMessage_Images_Val").val(null);
    $("#EditingOrReplying_Box").slideUp(300);
    if ($("#SendMessage_Text_Val").val().length <= 0) $(".btn-submit-msg-sending").attr("disabled", true);
    else $(".btn-submit-msg-sending").attr("disabled", false);
});
$("#EditDiscussionMessage_Btn").on("click", function () {
    let uncodedText = textUncoder($("#EditingOrReplyingMsgText_Lbl").html());
    insideBoxClose(false, "DiscussionOptions_Box");

    $("#SendMessage_Images_Val").val(null);
    $("#EditingOrReplyingMsgStatus_Lbl").text("Edit Message");
    $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-regular fa-pen-to-square"></i> ');
    $("#SendMessage_Text_Val").val(uncodedText);
    $(".send-message-variety").attr("action", "/DiscussionMessage/Edit");
    $(".send-message-variety").attr("id", "EditMessage_Form");

    setTimeout(function () {
        if ($("#EditingOrReplying_Box").css("display") == "block") {
            $("#EditingOrReplying_Box").slideUp(300);
            setTimeout(function () {
                $("#EditingOrReplying_Box").slideDown(300);
            }, 350);
        }
        else {
            $("#EditingOrReplying_Box").slideDown(300);
        }
    }, 400);
});
$("#ReplyDiscussionMessage_Btn").on("click", function () {
    insideBoxClose(false, "DiscussionOptions_Box");
    insideBoxClose(false, "ReactionsOptions_Container");

    //$("#SendMessage_Images_Val").val(null);
    $("#EditingOrReplyingMsgStatus_Lbl").text("Reply to");
    $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-solid fa-reply"></i> ');
    $(".send-message-variety").attr("action", "/DiscussionMessage/Reply");
    $(".send-message-variety").attr("id", "SendDiscussionReply_Form");

    setTimeout(function () {
        if ($("#EditingOrReplying_Box").css("display") == "block") {
            $("#EditingOrReplying_Box").slideUp(300);
            setTimeout(function () {
                $("#EditingOrReplying_Box").slideDown(300);
            }, 350);
        }
        else {
            $("#EditingOrReplying_Box").slideDown(300);
        }
    }, 400);
});

$("#DI_LoadAdditionalInfo_Btn").on("click", function () {
    $("#AdditionalInfo_Container").empty();

    let name = $("#DiscussionName_Lbl").html();
    let shortLink = $("#DiscussionShortlink_Lbl").html();
    let membersCount = $("#StatusBar_Lbl-0").html() + " out of 1200";
    let createdAt = $("#CreatedAt_Val").val();
    let isPrivate = $("#IsPrivate_Val").val();

    let linkContainer = $("<div class='bordered-container p-2 mt-2 text-center'></div>");
    let linkContainer_Icon = $("<h1 class='h1 text-primary'> <i class='fa-solid fa-link'></i> </h1>");
    let linkContainer_Header = $("<h5 class='h5'>Discussion Link</h5>");
    let link_NMContainer = $("<div class='box-container mt-2'></div>");
    let link_NM_Small = $("<small class='card-text text-muted'>Tap on the button to copy the link</small>");
    let link_ActualContainer = $("<div class='box-container bg-light p-2 mt-2'></div>");
    let link_ActualSpan = $("<span class='card-text' id='DiscussionLinkAbout_Span'></span>");
    let link_ActualCopyBtn = $("<button type='button' class='btn btn-light btn-standard copy-to-clipboard ms-2' id='DiscussionLinkAbout_Span-Copy'> <i class='fa-regular fa-copy text-primary'></i> Copy</button>");
    link_ActualSpan.html('/discussion/discuss/' + shortLink.substring(1, shortLink.length));

    link_ActualContainer.append(link_ActualSpan);
    link_ActualContainer.append(link_ActualCopyBtn);
    link_NMContainer.append(link_NM_Small);
    link_NMContainer.append(link_ActualContainer);
    linkContainer.append(linkContainer_Icon);
    linkContainer.append(linkContainer_Header);
    linkContainer.append(link_NMContainer);

    let nameDiv = $("<div class='p-2 pb-0'></div>");
    let nameTitle = $("<small class='card-text text-muted fw-500'>Name</small>");
    let nameSeparator = $("<div></div>");
    let nameLbl = $("<small class='card-text' id='AdditionalInfo_DiscussionName_Lbl'></small>");

    let shortlinkDiv = $("<div class='border-top mt-2 p-2 pb-0'></div>");
    let shortlinkTitle = $("<small class='card-text text-muted fw-500'>Shortlink</small>");
    let shortlinkSeparator = $("<div></div>");
    let shortlinkLbl = $("<small class='card-text' id='AdditionalInfo_DiscussionShortlink_Lbl'></small>");

    let membersCountDiv = $("<div class='border-top mt-2 p-2 pb-0'></div>");
    let membersCountTitle = $("<small class='card-text text-muted fw-500'>Number of Members</small>");
    let membersCountSeparator = $("<div></div>");
    let membersCountLbl = $("<small class='card-text' id='AdditionalInfo_DiscussionMembersCount_Lbl'></small>");

    let createdAtDiv = $("<div class='border-top mt-2 p-2 pb-0'></div>");
    let createdAtTitle = $("<small class='card-text text-muted fw-500'>Created at</small>");
    let createdAtSeparator = $("<div></div>");
    let createdAtLbl = $("<small class='card-text' id='AdditionalInfo_DiscussionCreatedAt_Lbl'></small>");

    let isPrivateDiv = $("<div class='border-top mt-2 p-2 pb-0'></div>");
    let isPrivateTitle = $("<small class='card-text text-muted fw-500'>Privacy</small>");
    let isPrivateSeparator = $("<div></div>");
    let isPrivateLbl = $("<small class='card-text'>Created at</small>");

    nameLbl.html(name);
    shortlinkLbl.html(shortLink);
    membersCountLbl.html(membersCount);
    createdAtLbl.html(dateAndTimeConverter(createdAt));
    if (isPrivate) isPrivateLbl.html("<span class='text-orange'> <i class='fa-solid fa-lock'></i> </span> Private Discussion; secured with password");
    else isPrivateLbl.html("<span class='text-primary'> <i class='fa-solid fa-lock-open'></i> </span> Free-to-enter discussion");

    nameDiv.append(nameTitle);
    nameDiv.append(nameSeparator);
    nameDiv.append(nameLbl);
    shortlinkDiv.append(shortlinkTitle);
    shortlinkDiv.append(shortlinkSeparator);
    shortlinkDiv.append(shortlinkLbl);
    membersCountDiv.append(membersCountTitle);
    membersCountDiv.append(membersCountSeparator);
    membersCountDiv.append(membersCountLbl);
    createdAtDiv.append(createdAtTitle);
    createdAtDiv.append(createdAtSeparator);
    createdAtDiv.append(createdAtLbl);
    isPrivateDiv.append(isPrivateTitle);
    isPrivateDiv.append(isPrivateSeparator);
    isPrivateDiv.append(isPrivateLbl);

    $("#AdditionalInfo_Container").append(linkContainer);
    $("#AdditionalInfo_Container").append(nameDiv);
    $("#AdditionalInfo_Container").append(shortlinkDiv);
    $("#AdditionalInfo_Container").append(membersCountDiv);
    $("#AdditionalInfo_Container").append(createdAtDiv);
    $("#AdditionalInfo_Container").append(isPrivateDiv);

    slideToRight("DiscussionAdditionalInfo_Container");
});

//Chat Messages//
$("#GetChatMessageInfo_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        if (response.success) {
            $("#EditingOrReplying_Box").slideUp(250);

            textDecoder(response.result.text, "ChatOptionMessageText_Lbl");
            $("#CurrentGotChatMsg_Id_Val").val(response.result.id);
            if (response.result.isEdited) $("#ChatMsgOptionAdditionalInfo_Lbl").text(dateAndTimeTranslator(response.result.sentAt) + ", edited");
            else $("#ChatMsgOptionAdditionalInfo_Lbl").text(dateAndTimeTranslator(response.result.sentAt));
            if (response.result.userId == response.userId) {
                $("#EditChatMsg_Col").fadeIn(150);
                $("#DeleteChatMsg_Col").fadeIn(150);
                $("#DDM_Id_Val").val(response.result.id);
                if (response.isEditable) {
                    $("#EditChatMessage_Btn").attr("disabled", false);
                }
                else $("#EditChatMessage_Btn").attr("disabled", true);

            }
            else {
                $("#EditChatMsg_Col").fadeOut(150);
                $("#DeleteChatMsg_Col").fadeOut(150);
            }

            setTimeout(function () {
                insideBoxOpen("ChatMessageOptions_Box");
            }, 250);
        }
        else {
            $("#CurrentGotChatMsg_Id_Val").val(0);
            alert('<i class="fa-regular fa-circle-xmark fa-shake" style="--fa-animation-duration: 1.75s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 3.5);
        }
    });
});

$("#CancelChatMessageSettings_Btn").on("click", function () {
    $("#EM_Id_Val").val(0);
    $("#RM_Id_Val").val(0);
    $("#DDM_Id_Val").val(0);
    $("#RM_ReplyText_Val").val("");
    $("#EditingOrReplying_Box").slideUp(250);
});
$("#EditChatMessage_Btn").on("click", function () {
    let trueId = $("#CurrentGotChatMsg_Id_Val").val();
    if (trueId != "" && parseInt(trueId) > 0) {
        let messageText = $("#" + trueId + "-ChatOptionMsgText_Lbl").text();
        messageText = textUncoder(messageText);
        $("#EM_Id_Val").val(trueId);
        $("#DDM_Id_Val").val(0);
        $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-regular fa-pen-to-square"></i> ');
        $("#EditingOrReplyingMsgStatus_Lbl").text("Edit Message");
        $("#EditingOrReplyingMsgText_Lbl").html(messageText);
        $("#SendMessage_Text_Val").val(messageText);

        insideBoxClose(false, "ChatMessageOptions_Box");
        setTimeout(function () {
            $("#EditingOrReplying_Box").slideDown(250);
        }, 450);
    }
    else {
        $("#EM_Id_Val").val(0);
        $("#DDM_Id_Val").val(0);
        $("#CurrentGotChatMsg_Id_Val").val(0);
    }
});
$("#ReplyChatMessage_Btn").on("click", function () {
    let trueId = $("#CurrentGotChatMsg_Id_Val").val();
    if (trueId != "") {
        let messageText = $("#" + trueId + "-ChatOptionMsgText_Lbl").html();
        if (messageText != "" || messageText != undefined) {

            messageText = messageText.length > 40 ? messageText.substring(0, 37) + "..." : messageText;

            $("#RM_ReplyId_Val").val(trueId);
            $("#DDM_Id_Val").val(0);
            $("#RM_ReplyText_Val").val(messageText);
            $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fas fa-reply"></i> ');
            $("#EditingOrReplyingMsgStatus_Lbl").text("Reply to Message");
            $("#EditingOrReplyingMsgText_Lbl").html(messageText);
            $("#SendMessage_Text_Val").val("");
            $("#SendMessage_SbmtBtn").attr("disabled", true);
            insideBoxClose(false, "ChatMessageOptions_Box");
            setTimeout(function () {
                $("#EditingOrReplying_Box").slideDown(250);
            }, 450);
        }
        else {
        }
    }
    else {
        $("#RM_ReplyId_Val").val(0);
        $("#DDM_Id_Val").val(0);
        $("#RM_ReplyText_Val").val("");
        $("#EditingOrReplying_Box").slideUp(250);
    }
});
//Chat Messages//

$(document).on("click", ".btn-get-discussion-info", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#GSI_Id_Val").val(trueId);
        $("#GetDiscussionShortInfo_Form").submit();
    }
    else $("#GSI_Id_Val").val(0);
});

$(document).on("click", ".reply-to-discussion-message", function (event) {
    let trueId = getTrueId(event.target.id);
    if ($("#" + event.target.id).hasClass("reply-element")) {
        trueId = parseInt($("#" + event.target.id).attr("data-bs-msg-id"));
    }

    $("#SendMessage_Images_Val").val(null);
    $("#EditingOrReplying_Box").slideUp(300);
    if (trueId != null && trueId > 0) {
        $("#EditDiscussionMsg_Col").fadeOut(300);
        $("#DeleteDiscussionMsg_Col").fadeOut(300);
        $("#DiscussionOptionAdditionalInfo_Lbl").fadeOut(100);
        $("#RM_ReplyId_Val").val(trueId);
        $("#SDMA_MessageId_Val").val(trueId);
        $("#SDMR_Id_Val").val(trueId);
        $("#GDMR_Id_Val").val(trueId);
        $("#GDMA_Id_Val").val(trueId);
        $("#CurrentGotDiscussionMsg_Id_Val").val(trueId);
        $("#GDMA_MsgText_Val").val($("#" + trueId + "-DiscussionOptionMsgText_Lbl").text().substring(0, 40));

        setTimeout(function () {
            $("#DiscussionOptionMessageText_Lbl").html($("#" + trueId + "-DiscussionOptionMsgText_Lbl").text());
            $("#RM_ReplyText_Val").val($("#" + trueId + "-DiscussionOptionMsgText_Lbl").html().substring(0, 40));
            $("#EditingOrReplyingMsgText_Lbl").text($("#" + trueId + "-DiscussionOptionMsgText_Lbl").text().substring(0, 40));

            textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "DiscussionOptionMessageText_Lbl");
            textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "EditingOrReplyingMsgText_Lbl");

            insideBoxOpen("DiscussionOptions_Box");
        }, 225);
    }
    else {
        $("#RM_ReplyId_Val").val(0);
        $("#RM_ReplyText_Val").val(null);
    }
});

$(document).on("click", ".btn-block-the-user-out-from-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#BU_UserId_Val").val(trueId);
        $("#BlockTheUserOutFromTheDiscussion_Form").submit();
    }
    else $("#BU_UserId_Val").val(0);
});
$(document).on("click", ".btn-unblock-the-user-out-from-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#UBU_UserId_Val").val(trueId);
        $("#UnblockTheUserOutFromTheDiscussion_Form").submit();
    }
    else $("#BU_UserId_Val").val(0);
});

$(document).on("click", ".pin-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#PinTheDiscussion_Id_Val").val(trueId);
        $("#PinTheDiscussion_Form").submit();
    }
    else $("#PinTheDiscussion_Id_Val").val(0);
});
$(document).on("click", ".unpin-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#UnpinTheDiscussion_Id_Val").val(trueId);
        $("#UnpinTheDiscussion_Form").submit();
    }
    else $("#UnpinTheDiscussion_Id_Val").val(0);
});
$(document).on("click", ".mute-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#MuteTheDiscussion_Id_Val").val(trueId);
        $("#MuteTheDiscussion_Form").submit();
    }
    else $("#MuteTheDiscussion_Id_Val").val(0);
});
$(document).on("click", ".unmute-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#UnmuteTheDiscussion_Id_Val").val(trueId);
        $("#UnmuteTheDiscussion_Form").submit();
    }
    else $("#UnmuteTheDiscussion_Id_Val").val(0);
});

$(document).on("click", ".add-member-to-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#AddMember_UserId_Val").val(trueId);
        $("#AddMemberToDiscussion_Form").submit();
    }
    else $("#AddMember_UserId_Val").val();
})
$(document).on("click", ".relocate-to-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        document.location.href = "/Discussion/Discuss/" + trueId
    }
});
$(document).on("click", ".relocate-to-chat", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        document.location.href = "/Chat/C/" + trueId;
    }
});

$(document).on("click", ".discussion-options", function (event) {
    let trueId = getTrueId(event.target.id);
    if ($("#" + event.target.id).hasClass("reply-element")) {
        trueId = parseInt($("#" + event.target.id).attr("data-bs-msg-id"));
    }

    if (trueId != null && trueId > 0) {
        $("#GMI_Id_Val").val(trueId);
        $("#GetMessageInfo_Form").submit();
    }
    else $("#GMI_Id_Val").val(0);
});
$(document).on("click", ".discussion-alternative-options", function (event) {
    let trueId = $("#" + event.target.id).attr("data-bs-html");
    if (trueId != undefined && trueId > 0) {
        $("#GMI_Id_Val").val(trueId);
        $("#GetMessageInfo_Form").submit();
    }
    else $("#GMI_Id_Val").val(0);
});
$(document).on("click", ".mark-as-read-discussion-message", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#MAR_Id_Val").val(trueId);
        $("#MarkasReadDiscussionMessage_Form").submit();
    }
    else $("#MAR_Id_Val").val(0);
});

$(document).on("click", ".btn-delete-user-from-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#DUFD_Id_Val").val(trueId);
        $("#DeleteUserFromDiscussion_Form").submit();
    }
    else $("#DUFD_Id_Val").val(0);
});//GetChats_Form
$(document).on("click", ".btn-edit-access-level", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let accessLevel = $(this).attr("data-bs-html");
        let changerId = $(this).attr("data-bs-title");
        if (parseInt(accessLevel) >= 0 && parseInt(accessLevel) < 2 && changerId != null) {
            $("#EAL_AccessLevel_Val").val(accessLevel);
            $("#EAL_ChangerId_Val").val(changerId);
            $("#EAL_UserId_Val").val(trueId);
            $("#ChangeAccessLevel_Form").submit();
        }
    }
    else {
        $("#EAL_AccessLevel_Val").val(0);
        $("#EAL_ChangerId_Val").val(0);
    }
});

$(document).on("click", ".select-to-restore-the-discussion", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#RestoreTheDiscussion_DiscussionId_Val").val(trueId);
        $("#RestoreTheDiscussion_Form").submit();
    }
    else $("#RestoreTheDiscussion_DiscussionId_Val").val(0);
});

$(document).on("click", ".get-notification-info", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#GNI_Id_Val").val(trueId);
        $("#GetNotificationInfo_Form").submit();
    }
    else $("#GNI_Id_Val").val(0);
});
$(document).on("click", ".notification-mark-as-read", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#MaR_Id_Val").val(trueId);
        $("#MarkTheNotificationAsRead_Form").submit();
    }
    else {
        $("#MaR_Id_Val").val(0);
    }
});
$(document).on("click", ".pin-the-notification", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#PN_Id_Val").val(trueId);
        $("#PinTheNotification_Form").submit();
    }
    else $("#PN_Id_Val").val(0);
});
$(document).on("click", ".unpin-the-notification", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#UPN_Id_Val").val(trueId);
        $("#UnpinTheNotification_Form").submit();
    }
    else $("#UPN_Id_Val").val(0);
});
$(document).on("click", ".delete-notification", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#DN_Id_Val").val(trueId);
        $("#DeleteNotification_Form").submit();
    }
    else $("#DN_Id_Val").val(0);
});

//Chat Message Options//
$(document).on("click", ".chat-options", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#CurrentGotChatMsg_Id_Val").val(trueId);
        $("#CGMI_Id_Val").val(trueId);
        $("#GetChatMessageInfo_Form").submit();
    }
    else {
        $("#CurrentGotChatMsg_Id_Val").val(0);
        $("#CGMI_Id_Val").val(0);
    }
});
//Chat MessageOptions//

$(document).on("click", ".set-autodelete-duration", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#SendMessage_IsAutoDeletable").val(trueId);
        $(".set-autodelete-duration").removeClass("bg-primary text-light");
        $(".set-autodelete-duration").addClass("bg-white");
        $("#" + trueId + "-MinsSet_Btn").removeClass("bg-white");
        $("#" + trueId + "-MinsSet_Btn").addClass("bg-primary text-light");

        let trueValue = dateAndTimeTranslatorFromMins(trueId);
        $("#AreMessagesAutoDeletable-Span").text(trueValue);
        $("#AutoDeleteDelay-Icon").text(trueValue);
        if (trueId > 0) {
            $("#AreMessagesAutoDeletable-Icon").removeClass("text-muted");
            $("#AreMessagesAutoDeletable-Icon").addClass("text-primary");
            $("#AreMessagesAutoDeletable-Icon").css("animation", "plus-change-animation 1.9s 1 linear");
            $("#AreMessagesAutoDeletable-Icon").html('<i class="fa-solid fa-clock-rotate-left"></i>');
            $("#AutoDeleteDelay-Icon").removeClass("text-muted");
            $("#AutoDeleteDelay-Icon").addClass("text-primary");
        }
        else {
            $("#AreMessagesAutoDeletable-Icon").removeClass("text-primary");
            $("#AreMessagesAutoDeletable-Icon").addClass("text-muted");
            $("#AreMessagesAutoDeletable-Icon").css("animation", "plus-change-animation 1.9s 1 linear");
            $("#AreMessagesAutoDeletable-Icon").html('<i class="fa-regular fa-circle-xmark"></i>');
            $("#AutoDeleteDelay-Icon").addClass("text-muted");
            $("#AutoDeleteDelay-Icon").removeClass("text-primary");
        }
    }
    else {
        $("#SendMessage_IsAutoDeletable").val(trueId);
        $(".set-autodelete-duration").removeClass("bg-primary text-light");
        $(".set-autodelete-duration").addClass("bg-white");
        $("#AreMessagesAutoDeletable-Span").text("disabled");
        $("#AreMessagesAutoDeletable-Icon").addClass("text-muted");
        $("#AreMessagesAutoDeletable-Icon").removeClass("text-primary open-animated-collapse");
        $("#AutoDeleteDelay-Icon").addClass("text-muted");
        $("#AutoDeleteDelay-Icon").removeClass("text-primary");
    }
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
$(document).on("click", ".btn-open-inside-box", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $(".messages-container").css("margin-bottom", "-300px");
        $(".messages-container").fadeOut(300);
        setTimeout(function () {
            insideBoxOpen(trueId);
        }, 100);
    }
});
$(document).on("click", ".btn-close-inside-box", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        insideBoxClose(false, trueId);
    }
});
$(document).on("click", ".btn-slide-to-right", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        slideToRight(trueId);
    }
});
$(document).on("click", ".btn-slide-to-left", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        slideToLeft(trueId);
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

$(document).on("click", ".btn-execute-cmd", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let command = $(this).attr("data-bs-html");
        if (command != "" || command != undefined) {
            $("#SendMessage_Text_Val").val(command);
            $("#SendLiveDiscussionMessage_Text_Val").val(command);
            $("#SendChatMessage_Text_Val").val(command);

            $("#SendMessage_Text_Val").change();
            $("#SendLiveDiscussionMessage_Text_Val").change();
            $("#SendChatMessage_Text_Val").change();
        }
    }
});

$(document).on("click", ".copy-to-clipboard", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        navigator.clipboard.writeText($("#" + trueId).html());
        alert('<i class="fa-regular fa-copy text-neon-purple"></i>', "Copied to clipboard successfully", "Done", null, 0, null, null, null, 3.5);
        insideBoxClose(true, null);
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

$(document).on("click", ".set-the-icon", function (event) {
    let iconValue = $(this).html();
    if (iconValue != "") {
        $("#EditAvatarDesign_AvatarStickerUrl_Val").val(iconValue);
        $("#UnpicturedAvatarSm_Lbl").html(" " + iconValue + " ");
        $("#AvatarStyleChange_MainLbl").html(" " + iconValue + " ");
    }
});

$(document).on("click", ".set-style", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != "") {
        $("#TextType_Val").val(trueId);
        switch (parseInt(trueId)) {
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

        animatedOpen(false, "TextEditorSettings_Container", true, true);
    }
});

$(document).on("click", ".delete-message-options", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        insideBoxClose(false, trueId);
        setTimeout(function () {
            $(".got-msg-text").html("Message Text");
            $(".got-msg-datetime").html("sent date, time, is checked and edited");
            $(".got-msg-id-value").val(0);
        }, 450);
    }
});

$(document).on("click", ".set-disable-duration", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $(".set-disable-duration").removeClass("border-primary");
        $(this).addClass("border-primary");
        $("#DC_DurationInMins_Val").val(trueId);
        $("#DisableChatSbmt_Btn").attr("disabled", false);
    }
    else {
        $("#DC_DurationInMins_Val").val(0);
        $("#DisableChatSbmt_Btn").attr("disabled", true);
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

$("#SetAvatarChar_Btn").on("click", function () {
    let char = $("#AvatarChar").val();
    let compiledChar;
    let fontWeight = $("#FontWeight_Val").val();
    let rotationValue = $("#Rotation_Val").val();
    let fontFamily = $("#FontFamily_Val").val();
    if (char != "") {
        if (rotationValue > 0) {
            compilledClass = "transform:rotate(" + rotationValue + "deg);";
            compiledChar = "<div class='" + fontFamily + "'style = 'transform:rotate(" + rotationValue + "deg);font-weight:'" + fontWeight + ";'>" + char + "</div>";
        }
        else {
            compiledChar = "<div class='" + fontFamily + "'style = 'font-weight:" + fontWeight + ";'>" + char + "</div>";
        }

        $("#EditAvatarDesign_AvatarStickerUrl_Val").val(compiledChar);
        $("#UnpicturedAvatarSm_Lbl").html(compiledChar);
        $("#AvatarStyleChange_MainLbl").html(compiledChar);
    }
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

$("#SelectaFile_Val").on("change", function () {
    let fileInfo = $(this).get(0).files;
    if (fileInfo != null) {
        $("#SelectedFileInfo_Lbl").text(fileInfo[0].name + ", in " + fileInfo.length + " quantity(ies)");
    }
});
$("#SendAFileInMessage_Btn").on("click", function () {
    let id = $(this).attr("data-bs-html");
    if (id != null) $("#" + id).click();
});

$(document).on("click", ".set-font", function () {
    let fontFamily = $(this).attr("data-bs-font");
    let setTo = $(this).attr("data-bs-set-in");
    if (fontFamily != "null" && setTo != "") {
        $("#" + setTo).val(fontFamily);
    }
    else {
        $("#" + setTo).val(null);
    }
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
        $("#" + trueId).val(replaceAllUsersInText($("#" + trueId).val()));
        //$("#" + trueId).val(clearAllLinksInText($("#" + trueId).val()));
        //$("#" + trueId).val(replaceAllLinksInText($("#" + trueId).val()));
        textDecoder($("#" + trueId).val(), pasteTo);
        $("#" + trueId).val("");
    }
});
$(document).on("click", ".text-editor", function (event) {
    let elementId = getTrueId(event.target.id);
    let pasteTo = $("#" + event.target.id).attr("data-bs-paste-to");

    if (elementId != undefined && pasteTo != undefined) {
        textEditor(elementId, null, null, pasteTo);
    }
});

$(document).on("click", ".btn-file-click", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#" + trueId).click();
    }
});
$(document).on("change", ".send-message-files", function (event) {
    let trueId = event.target.id;
    if (trueId != undefined) {
        let files = $("#" + trueId).get(0).files;
        let filesFullName = "";

        if (files.length > 0 && files.length <= 6) {
            insideBoxClose(false, "DiscussionOptions_Box");
            for (let i = 0; i < files.length; i++) {
                filesFullName += files[i].name + ", ";
            }
            filesFullName = filesFullName.substring(0, filesFullName.lastIndexOf(","));
            $("#EditingOrReplyingMsgStatus_Lbl").text(files.length + " Images Selected");
            $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-regular fa-images text-primary"></i> ');
            $("#EditingOrReplyingMsgText_Lbl").text(filesFullName);
            $("#EditingOrReplying_Box").slideDown(250);
            $(".btn-submit-msg-sending").attr("disabled", false);
        }
        else if (files.length > 6) {
            $(".btn-submit-msg-sending").attr("disabled", true);
            $("#EditingOrReplyingMsgStatus_Lbl").text(files.length + " Images Selected");
            $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-regular fa-images text-primary"></i> ');
            $("#EditingOrReplyingMsgText_Lbl").html("Max images count per single message is restricted by <span class='fw-500'>6</span>. All other images will not be sent");
            $("#EditingOrReplying_Box").slideDown(250);

        }
        else {
            $("#SendMessage_Images_Val").val(null);
            $("#EditingOrReplying_Box").slideUp(250);
        }
    }
    else {
        $("#SendMessage_Images_Val").val(null);
        $("#EditingOrReplyingMsgStatus_Lbl").text("Edit Message");
        $("#EditingOrReplyingMsgIcon_Lbl").html(' <i class="fa-regular fa-pen-to-square"></i> ');
        $("#EditingOrReplyingMsgText_Lbl").text("Message Text...");
    }
});

$(document).on("click", ".discussion-header", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        headerTransform(trueId);
    }
});
$(document).on("click", ".discussion-header-return", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let mainBoxElement = trueId.substring(0, trueId.lastIndexOf("_"));
        headerReturn(trueId);
        setTimeout(function () {
            slideToTheBottom(mainBoxElement);
        }, 300);
    }
});

$(document).on("click", ".btn-hide", function (event) {
    let trueId = getTrueId(event.target.id);
    let secondId = $("#" + event.target.id).attr("data-bs-title");
    if (trueId != null) {
        if (secondId != undefined) hide(trueId, secondId);
        else hide(trueId, null);
    }
});

$(document).on("keyup", ".send-message-form-control", function () {
    let value = $(this).val();
    if (value[0] == "/") $("#SendMessage_SbmtBtn").attr("disabled", true);
    if (value.length > 0) {
        $("#PreviewTheMessage_Btn").fadeIn(250);
    }
    else $("#PreviewTheMessage_Btn").fadeOut(250);
    textEditorIndicators(value, "TextEditorIndicators_Box");
});
$(document).on("change", ".send-message-form-control", function (event) {
    let trueId = event.target.id;
    if (trueId != null) {
        let value = $(this).val();
        if (value[0] == "/") {
            let currentCommand;
            if (value.includes(" ")) currentCommand = (value.substring(0, value.indexOf(" "))).toLowerCase();
            else currentCommand = (value.substring(0, value.length)).toLowerCase();

            $("#CreatedAdditionalCommands_Box").fadeOut(350);
            $("#AdditionalCommands_Box").fadeOut(350);

            $("#CurrentCommand_Lbl").text(currentCommand);
            commandLineOptions(currentCommand);

            if (parseInt($("#CommandLine_Box").css("margin-bottom")) > -1200) {
                insideBoxClose(false, "CommandLine_Box");
                setTimeout(function () {
                    insideBoxOpen("CommandLine_Box");
                }, 750);
            }
            else {
                insideBoxOpen("CommandLine_Box");
            }
        }
    }
});

$(document).on("click", ".user-via-shortname", function () {
    let trueName = $(this).text().substring(1);
    if (trueName != null) {
        $("#FBD_Shortname_Val").val(trueName);
        $("#FindUserByShortname_Form").submit();
    }
    else $("#FBD_Shortname_Val").val(null);
});
$(document).on("click", ".user-via-id", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#FBI_Id_Val").val(trueId);
        $("#FindUserById_Form").submit();
    }
    else $("#FBI_Id_Val").val(null);
});

$(document).on("click", ".text-via-link", function (event) {
    let link = $(this).html();
    if (link != "" || link != undefined) {
        window.location.href = link;
    }
    else alert('<i class="fa-solid fa-link-slash fa-shake" style="--fa-animation-iteration-count: 1; --fa-animation-duration: 1.25s;"></i>', "Wrong link. We cannot open a page with that link, maybe because it's unsafe or the exact link is not correct", null, null, 0, null, null, null, 3.25);
});

$(document).on("click", ".btn-set-status", function (event) {
    botNavbarClose(true, null);
    setTimeout(function () {
        botNavbarOpen("StatusEdit_BotOffNavbar", 'Preloaded_BotOffNavbar', '<i class="fa-regular fa-keyboard"></i>');
    }, 100);
});

$(document).on("click", ".return-main-bot-navbar", function (event) {
    botNavbarClose(true, null);

    botOffNavbarH = $("#MainBotOffNavbar").innerHeight();
    let topNavbarH = $(".top-navbar").innerHeight();
    let neededH = fullHeight - 24 - botOffNavbarH - topNavbarH;
    $(".mh-max").css("height", neededH + "px");
    $(".main-container").css("bottom", botOffNavbarH + 1 + "px");
    $(".main-container").css("max-height", neededH + 1 + "px");
    $(".messages-container").css("bottom", botOffNavbarH + 8 + "px");

    $("#FifthMain_Btn").fadeIn(350);
    setTimeout(function () {
        botNavbarOpen("MainBotOffNavbar");
    }, 100);
});
$(document).on("click", ".open-bot-off-navbar", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        let reservedBtn = $("#" + event.target.id).attr("data-bs-title");
        let btnHtml = $("#" + event.target.id).attr("data-bs-html");
        botNavbarClose(true, null);

        setTimeout(function () {
            botNavbarOpen(trueId, reservedBtn, btnHtml);
        }, 125);
    }
});
$(document).on("click", ".open-main-bot-navbar", function () {
    let trueId = $("#FifthMain_Btn").attr("data-bs-html");
    if (trueId != null) {
        botNavbarClose(true, null);

        botOffNavbarH = $("#" + trueId).innerHeight();
        let topNavbarH = $(".top-navbar").innerHeight();
        let neededH = fullHeight - 24 - botOffNavbarH - topNavbarH;
        $(".mh-max").css("height", neededH + "px");
        $(".main-container").css("bottom", botOffNavbarH + 1 + "px");
        $(".main-container").css("max-height", neededH + 1 + "px");
        $(".messages-container").css("bottom", botOffNavbarH + 8 + "px");
        $("#FifthMain_Btn").fadeOut(350);

        setTimeout(function () {
            botNavbarOpen(trueId);
        }, 100);
    }
});

function getTrueId(name) {
    let trueId = name.substring(0, name.lastIndexOf("-"));
    if (trueId != null || trueId != undefined) return trueId;
    else return null;
}

function openSidebar() {
    if (fullWidth < 768) {
        $(".smallside-bot-navbar").css("bottom", "-300px");
        $(".smallside-bot-navbar").fadeIn(0);
        $(".btn-fixed-close-sidebar").fadeIn(300);
        $("#Main_SideBar").fadeIn(250);
        $("#Main_SideBar").css("left", 0);
        setTimeout(function () {
            $(".smallside-bot-navbar").css("bottom", 0);
        }, 300);
    }
}

function closeSidebar() {
    if (fullWidth < 768) {
        $(".smallside-bot-navbar").css("bottom", "-300px");
        animatedClose(true, "smallside-box-container ", true, true);
        $(".btn-fixed-close-sidebar").fadeOut(300);
        setTimeout(function () {
            $("#Main_SideBar").fadeOut(350);
            $("#Main_SideBar").css("left", "-1200px");
            $(".smallside-bot-navbar").fadeOut(0);
        }, 550);
    }
}

function ytVideoOptimizer(link) {
    let trueLink;
    if (link != null) {
        if (link.toLowerCase().includes("/embed")) {
            trueLink = link;
        }
        else if (link.toLowerCase().includes("watch")) {
            if (link.includes("&")) link = link.substring(link.indexOf("?") + 3, link.indexOf("&"));
            else link = link.substring(link.indexOf("?") + 3);
            trueLink = "https://www.youtube.com/embed/" + link;
        }
        else {
            link = link.substring(link.lastIndexOf("/") + 1);
            trueLink = "https://www.youtube.com/embed/" + link;
        }

        $("#YoutubeMiniPlayer_Frame").attr("src", trueLink);
        $("#YTNoCurrentView_Box").fadeOut(0);
        $("#YTMiniPlayer_Box").fadeIn(0);
    }
    else {
        $("#YoutubeMiniPlayer_Frame").attr("src", null);
        $("#YTNoCurrentView_Box").fadeIn(0);
        $("#YTMiniPlayer_Box").fadeOut(0);
    }
}

function commandLineOptions(command) {
    $(".command-box").fadeOut(300);
    switch (command) {
        case "/info":
            $("#CommandsInfo_Box").fadeIn(300);
            $("#CreateCommand_Box").fadeOut(300);
            break;
        case "/create":
            $("#CreateCommand_Box").fadeIn(300);
            $("#CommandsInfo_Box").fadeOut(300);
            break;
        case "/get":
            $("#GetDiscussionCommands_Form").submit();
            break;
        case "/get_commands":
            $("#GetDiscussionCommands_Form").submit();
            break;
        default:
            $("#CommandsInfo_Box").fadeIn(300);
            $("#CreateCommand_Box").fadeOut(300);
            break;
    }
}

function hide(element, openAnotherElement) {
    if (openAnotherElement == null) {
        $("#" + element).fadeOut(350);
    }
    else {
        $("#" + element).fadeOut(225);
        setTimeout(function () {
            $("#" + openAnotherElement).fadeIn(250);
        }, 225);
    }
}

function headerTransform(element) {
    $("#" + element).css("filter", "none");
    $(".special-container-header").slideUp(300);
    $(".postheader-container").slideUp(240);
    $(".discussion-box").slideUp(300);
    insideBoxClose(true, null);
    setTimeout(function () {
        $("#" + element).fadeIn(0);
        $("#" + element).css("margin-left", 0);
    }, 350);
}
function headerReturn(element) {
    $("#" + element).css("filter", "blur(16px)");
    $("#" + element).css("margin-left", "-1200px");
    setTimeout(function () {
        $("#" + element).fadeOut(0);
    }, 250);
    setTimeout(function () {
        $(".postheader-container").fadeIn(250);
        $(".special-container-header").fadeIn(250);
        $(".discussion-box").fadeIn(250);
    }, 300);
}

function slideToLeft(element) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("margin-left", "28px");
    $("#" + element).css("filter", "blur(9px)");
    setTimeout(function () {
        $("#" + element).css("margin-left", "-1200px");
    }, 400);
    setTimeout(function () {
        $("#" + element).fadeOut(0);
        $("#" + element).css("filter", "none");
    }, 650);

    //$("#" + element).css("background-color", "transparent");
    //$("#" + element).css("backdrop-filter", "blur(9px)");
    //setTimeout(function () {
    //    $("#" + element).css("transition", "all ease 0.5s");
    //    $("#" + element).css("margin-left", "-1200px");
    //}, 350);
    //setTimeout(function () {
    //    $("#" + element).fadeOut(450);
    //}, 600);
}

function setUrgencyIcon(value) {
    switch (parseInt(value)) {
        case 1:
            value = "<i class=\"fa-solid fa-circle text-danger\"></i>";
            break;
        case 2:
            value = "<i class=\"fa-solid fa-circle text-warning\"></i>";
            break;
        case 3:
            value = "<i class=\"fa-solid fa-circle text-neon-purple\"></i>";
            break;
        case 4:
            value = "<i class=\"fa-solid fa-circle text-primary\"></i>";
            break;
        case 5:
            value = "<i class=\"fa-solid fa-circle text-orange\"></i>";
            break;
        default:
            value = "<i class=\"fa-solid fa-circle text-muted\"></i>";
            break;
    }

    return value;
}

function dateAndTimeConverter(value) {
    if (value != null) {
        let monthsArray = ["january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december"];
        let dateAndTime = new Date(value);
        let currentDate = new Date();

        let day = dateAndTime.getMonth() + 1;
        let month = dateAndTime.getDate() - 1;
        let year = dateAndTime.getFullYear();

        let hours = dateAndTime.getHours() < 10 ? "0" + dateAndTime.getHours() : dateAndTime.getHours();
        let mins = dateAndTime.getMinutes() < 10 ? "0" + dateAndTime.getMinutes() : dateAndTime.getMinutes();
        if (currentDate.getFullYear() != year) {
            if (hours == "00" && mins == "00") {
                return day + " " + monthsArray[month] + " " + year;
            }
            else return day + " " + monthsArray[month] + " " + year + ", at " + hours + ":" + mins;
        }
        else {
            if (hours == "00" && mins == "00") {
                return day + " " + monthsArray[month];
            }
            else return day + " " + monthsArray[month] + ", at " + hours + ":" + mins;
        }
    }
    else return null;
}

function dateAndTimeTranslatorFromMins(minsValue) {
    let midValue = 0;
    if (minsValue >= 60 && minsValue < 1440) {
        midValue = Math.round(minsValue / 60);
        if (midValue == 1) return midValue + " hour";
        else return midValue + " hours";
    }
    else if (minsValue >= 1440 && minsValue < 10080) {
        midValue = Math.round((minsValue / 60) / 24);
        if (midValue == 1) return midValue + " day";
        else return midValue + " days";
    }
    else if (minsValue >= 10080 && minsValue < 43200) {
        midValue = Math.round((minsValue / 60) / 24 / 7);
        if (midValue == 1) return midValue + " week";
        else return midValue + " weeks";
    }
    else if (minsValue >= 43200) {
        midValue = Math.round((minsValue / 60) / 24 / 30);
        if (midValue == 1) return midValue + " month";
        else return midValue + " months";
        return Math.round((MinsValue / 60) / 24 / 30) + " month(s)";
    }
    else if (minsValue <= 0) return " disabled";
    else {
        return minsValue == 1 ? minsValue + " min" : minsValue + " mins";
    }
}

function dateAndTimeTranslator(value) {
    if (value != null) {
        let dateAndTime = new Date(value);
        let currentDateAndTime = new Date();
        let result = currentDateAndTime - dateAndTime;

        let yearsInResult = 0;
        let monthsInResult = 0;
        let daysInResult = parseInt(Math.round(result / (1000 * 60 * 60 * 24)));
        let finalResult;
        if (daysInResult >= 30) {
            monthsInResult = Math.round(daysInResult / 30);
            finalResult = monthsInResult + " m ago";
        }
        else if (daysInResult >= 365) {
            yearsInResult = Math.round(daysInResult / 365);
            finalResult = yearsInResult + " y ago";
        }
        else {
            let hours = dateAndTime.getHours() < 10 ? "0" + dateAndTime.getHours() : dateAndTime.getHours();
            let mins = dateAndTime.getMinutes() < 10 ? "0" + dateAndTime.getMinutes() : dateAndTime.getMinutes();

            if (daysInResult == 0) finalResult = "today, at " + hours + ":" + mins;
            else if (daysInResult == 1) finalResult = "yesterday, at " + hours + ":" + mins;
            else finalResult = daysInResult + " d ago, at " + hours + ":" + mins;
        }
        return finalResult;
    }
    else return null;
}

function valueToHex(value) {
    let hexValue = value.toString(16);
    return hexValue.length == 1 ? "0" + hexValue : hexValue;
}

function rgbToHex(red, green, blue) {
    return valueToHex(red) + valueToHex(green) + valueToHex(blue);
}

function deleteAllEndEmptyChars(value) {
    if (value[value.length - 1] == " ") {
        let notEmptyIndex = 0;
        for (let i = value.length - 1; i >= 0; i--) {
            if (value[i] != " ") {
                notEmptyIndex = i;
                break;
            }
        }
        value = value.substring(0, notEmptyIndex + 1);
    }

    return value;
}

function textEditor(elementId, preparedText1, preparedText2, pasteTo) {
    let element = parseInt(elementId);
    //set-style
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
        selectedText = fullText.substring(cursorStartPos, cursorEndPos);
        part2 = fullText.substring(cursorEndPos, fullText.length);
    }

    switch (element) {
        case 0:
            if (selectedText == null) {
                fullText = part1 + "[[]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[[" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 1:
            if (selectedText == null) {
                fullText = part1 + "[{]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[{" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 2:
            if (selectedText == null) {
                fullText = part1 + "[_]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[_" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 3:
            if (selectedText == null) {
                fullText = part1 + "[$]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[$" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
            }
            break;
        case 4:
            if (selectedText == null) {
                fullText = part1 + "[[ [!='color:" + preparedText1 + "'>" + preparedText2 + "]]" + part2;
                cursorStartPos = part1.length + 14 + preparedText1.length + preparedText2.length;
            }
            else {
                fullText = part1 + "[[ [!='color:" + preparedText1 + "' " + selectedText + "]]" + part2;
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
                fullText = part1 + "!%='" + preparedText1 + "'>" + selectedText + "!]" + part2;
                cursorStartPos = part1.length + 7 + part2.length;
            }
            break;
        case 8:
            if (selectedText == null) {
                fullText = part1 + "[/]]" + part2;
                cursorStartPos = part1.length + 2;
            }
            else {
                fullText = part1 + "[/" + selectedText + "]]" + part2;
                cursorStartPos = part1.length + 4 + selectedText.length;
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

function textEditorIndicators(value, setTo) {
    if (value.includes("[[")) {
        if ($("#BoldText_Indicator").length === 0) {
            let boldTextSpan = "<div class='indicator-badge bg-neon-purple' id='BoldText_Indicator'></div>";
            $("#" + setTo).append(boldTextSpan);
        }
    }
    else {
        $("#BoldText_Indicator").remove();
    }

    if (value.includes("[{")) {
        if ($("#ItalicText_Indicator").length === 0) {
            let italicTextSpan = "<div class='indicator-badge bg-primary' id='ItalicText_Indicator'></div>";
            $("#" + setTo).append(italicTextSpan);
        }
    }
    else {
        $("#ItalicText_Indicator").remove();
    }

    if (value.includes("[_")) {
        if ($("#UnderlinedText_Indicator").length === 0) {
            let underlinedTextSpan = "<div class='indicator-badge bg-success' id='UnderlinedText_Indicator'></div>";
            $("#" + setTo).append(underlinedTextSpan);
        }
    }
    else {
        $("#UnderlinedText_Indicator").remove();
    }

    if (value.includes("[$")) {
        if ($("#StyleFontText_Indicator").length === 0) {
            let styleFontTextSpan = "<div class='indicator-badge bg-danger' id='StyleFontText_Indicator'></div>";
            $("#" + setTo).append(styleFontTextSpan);
        }
    }
    else {
        $("#StyleFontText_Indicator").remove();
    }

    if (value.includes("[!")) {
        if ($("#ColoredText_Indicator").length === 0) {
            let styleFontTextSpan = "<div class='indicator-badge bg-gradient-one' id='ColoredText_Indicator'></div>";
            $("#" + setTo).append(styleFontTextSpan);
        }
    }
    else {
        $("#ColoredText_Indicator").remove();
    }

    if (value.includes("!$")) {
        if ($("#LinkedText_Indicator").length === 0) {
            let styleFontTextSpan = "<div class='indicator-badge bg-orange' id='LinkedText_Indicator'></div>";
            $("#" + setTo).append(styleFontTextSpan);
        }
    }
    else {
        $("#TooltipText_Indicator").remove();
    }

    if (value.includes("!%")) {
        if ($("#LinkedText_Indicator").length === 0) {
            let styleFontTextSpan = "<div class='indicator-badge bg-gradient-cute' id='LinkedText_Indicator'></div>";
            $("#" + setTo).append(styleFontTextSpan);
        }
    }
    else {
        $("#LinkedText_Indicator").remove();
    }

    if (value.includes("[/")) {
        if ($("#CommandLineText_Indicator").length === 0) {
            let styleFontTextSpan = "<div class='indicator-badge bg-secondary' id='CommandLineText_Indicator'></div>";
            $("#" + setTo).append(styleFontTextSpan);
        }
    }
    else {
        $("#CommandLineText_Indicator").remove();
    }
}

function statusSlider(mainId, maxCount) {
    if (maxCount > 0) {
        let currentCount = 0;
        setInterval(function () {
            $(".status-slider").fadeOut(250);
            setTimeout(function () {
                $("#" + mainId + "-" + currentCount).fadeIn(250);
            }, 225);
            currentCount++;
            if (currentCount > maxCount) currentCount = 0;
        }, 3500);
    }
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
    text = text.replaceAll("[/", "<span class='blurred-lines'>");
    text = text.replaceAll("]]", "</span>");
    text = text.replaceAll("*]", "</span>");
    text = text.replaceAll("$}", "</span>");
    text = text.replaceAll("[*", "<span class='user-via-shortname'>");
    text = text.replaceAll("!]", "</button>");
    text = text.replaceAll("$!", "</a>");
    text = text.replaceAll("{$", "<span class='text-via-link'>");

    $("#" + setIn).html(text);

    return text;
}

function textUncoder(text) {
    text = text.replaceAll("<span>", "[#");
    text = text.replace('<span class="fw-500">', "[[");
    text = text.replaceAll('span class="fst-italic">', "[{");
    text = text.replaceAll('<span class="text-decoration-underline">', "[_");
    text = text.replaceAll('<span class="style-font" >', "[$");
    text = text.replaceAll("<span style", "[!");
    text = text.replaceAll('<a class="text-decoration-none text-primary" href', "!$");
    text = text.replaceAll('<span class="card-text info-popover" data-bs-toggle="popover" data-bs-custom-class="tooltip-asset-one shadow-sm p-0" data-bs-placement="top" data-bs-container=vbody" data-bs-content', "!%");
    text = text.replaceAll('<span class="blurred-lines" >', "[/");
    text = text.replaceAll("</span>", "]]");
    text = text.replaceAll("</span>", "*]");
    text = text.replaceAll("</span>", "$}");
    text = text.replaceAll('<span class="user-via-shortname">', "[*");
    text = text.replaceAll("</button>", "!]");
    text = text.replaceAll("</a>", "$!");
    text = text.replaceAll('<span class="text-via-linkv">', "{$");

    return text;
}

function replaceAllTheTextInMessages() {
    let allTheText = document.getElementsByClassName("message-label");
    if (allTheText.length > 0) {
        for (let i = 0; i < allTheText.length; i++) {
            textDecoder(document.getElementById(allTheText[i].id).innerHTML, allTheText[i].id);
        }
    }
}

function setBackAllUsersInText(element) {
    let reservedText = $("#" + element).attr("data-bs-html");
    if (reservedText != undefined) {
        return reservedText;
    }
    else return null;
}

function clearAllLinksInText(text) {
    text = text.replaceAll("{$", '');
    text = text.replaceAll("$}", '');

    return text;
}

function replaceAllLinksInText(text) {
    return null;
}

function replaceAllUsersInText(text) {
    if (text != null) {
        let separatorsArray = [" ", ",", ".", ":", ";", "?", "!", "#", "$", "%", "^", "&", "*", "(", ")"];
        let usersArray = [];

        for (let i = 0; i < text.length; i++) {
            if (text[i] == "@") {
                for (let j = i; j < text.length; j++) {
                    for (let k = 0; k < separatorsArray.length; k++) {
                        let userLink;
                        if (text[j] == separatorsArray[k]) {
                            let separatorIndex = text.indexOf(separatorsArray[k], j);

                            if (separatorIndex > 0) {
                                userLink = text.substring(i, separatorIndex);
                            }
                            else userLink = text.substring(i, text.length);
                            usersArray.push(userLink);

                            j = text.length;
                            break;
                        }
                    }
                }
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
        $("#FirstReserve_Btn").html(' <i class="fa-solid fa-bars text-muted"></i> <br/><small>Menu</small>');
        $("#FirstReserve_Btn").addClass("open-sidebar");
        $("#FirstMain_Btn").fadeOut(0);
        $("#FirstReserve_Btn").fadeIn(0);
    }
}

function slideToTheBottom(element) {
    let preloadedDiv = document.getElementById(element);
    preloadedDiv.scroll({ top: preloadedDiv.scrollHeight, behavior: 'smooth' });
}

function slide(forAll, element) {
    $(".slider-container").css("opacity", "0.0");
    setTimeout(function () {
        $(".slider-container").fadeOut(0);
        $("#" + element).css("opacity", "1.0");
        $("#" + element).fadeIn(0);
    }, 300);
}

function slideToRight(element) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("margin-left", "28px");
    setTimeout(function () {
        $("#" + element).css("margin-left", 0);
    }, 350);
}

function botNavbarHeightUpdate() {
    let botOffNavbars = document.getElementsByClassName("bot-navbar");
    for (let i = 0; i < botOffNavbars.length; i++) {
        if (parseInt($("#" + botOffNavbars[i].id).css("bottom") >= 0)) {
            botOffNavbarH = $("#" + botOffNavbarH[i].id).innerHeight();
            break;
        }
    }

    return botOffNavbarH;
}

function botNavbarClose(isForAll, element) {
    if (isForAll) {
        $(".bot-navbar").css("bottom", "-1200px");
        $(".bot-navbar").fadeOut(350);
    }
    else {
        $("#" + element).css("bottom", "-1200px");
        $("#" + element).fadeOut(350);
    }
}
function botNavbarOpen(element, reservedBtn, reservedBtnHtml) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("bottom", 0);
    if (reservedBtn != null) {

        if (reservedBtnHtml != null) $("#FifthMain_Btn").html(reservedBtnHtml);
        else $("#FifthMain_Btn").html(' <i class="fa-solid fa-angle-left"></i> ');

        $("#FifthMain_Btn").attr("data-bs-html", reservedBtn);
        $("#FifthMain_Btn").fadeIn(350);
    }
}

function headerScrollTransformation(mainElement) {
    $("#" + mainElement + "-Header").css("position", "fixed");
    $("#" + mainElement + "_PostHeader").css("position", "fixed");
    $("#" + mainElement + "-Header").css("width", $("#" + mainElement).innerWidth() + "px");
    $("#" + mainElement + "_PostHeader").css("width", $("#" + mainElement).innerWidth() + "px");
    if (fullWidth < 768) $("#" + mainElement + "_PostHeader").css("top", $("#" + mainElement + "-Header").innerHeight() * 2.22 + "px");
    else if (fullWidth >= 768 && fullWidth < 1024) $("#" + mainElement + "_PostHeader").css("top", $("#" + mainElement + "-Header").innerHeight() * 2.1225 + "px");
    else if (fullWidth >= 1024 && fullWidth < 1367) $("#" + mainElement + "_PostHeader").css("top", $("#" + mainElement + "-Header").innerHeight() * 2.1345 + "px");
    else $("#" + mainElement + "_PostHeader").css("top", $("#" + mainElement + "-Header").innerHeight() * 2.14 + "px");
}
function headerScrollReturn(mainElement) {
    $("#" + mainElement + "-Header").css("width", "auto");
    $("#" + mainElement + "-Header").css("position", "relative");
    $("#" + mainElement + "_PostHeader").css("width", "auto");
    $("#" + mainElement + "_PostHeader").css("position", "relative");
    $("#" + mainElement + "_PostHeader").css("top", 0);
}

function verySmallBoxClose(element) {
    $("#" + element).css("margin-bottom", "14px");
    setTimeout(function () {
        $("#" + element).css("margin-bottom", 0);
        $("#" + element).fadeOut(0);
    }, 400);
}

function verySmallBoxOpen(element) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("margin-bottom", "12px");
    setTimeout(function () {
        $("#" + element).css("margin-bottom", "0");
    }, 400);
}

function insideBoxOpenByHeight(element, height) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("z-index", "0");
    $("#" + element).css("margin-bottom", height + 8 + "px");
    setTimeout(function () {
        $("#" + element).css("z-index", "1001");
    }, 225);
}

function insideBoxOpen(element) {
    let timeOut = 0;
    let chatBarHeight = botOffNavbarH - 90;
    let botOffNavbars = document.getElementsByClassName("bot-navbar");
    let messagesContainers = document.getElementsByClassName("messages-container");
    for (let i = 0; i < botOffNavbars.length; i++) {
        if ($("#" + botOffNavbars[i].id).innerHeight() - 75 > chatBarHeight) chatBarHeight = $("#" + botOffNavbars[i].id).innerHeight() - 75;
    }

    for (let i = 0; i < messagesContainers.length; i++) {
        if (parseInt($("#" + messagesContainers[i].id).css("margin-bottom")) > -1200) {
            $(".messages-container").css("margin-bottom", "-300px");
            timeOut = 125;
            break;
        }
        else timeOut = 0;
    }

    if (timeOut > 0) {
        setTimeout(function () {
            $("#" + element).fadeIn(0);
            $("#" + element).css("z-index", "0");
            $("#" + element).css("margin-bottom", chatBarHeight + 20 + "px");
            setTimeout(function () {
                $("#" + element).css("z-index", "1001");
                $("#" + element).css("margin-bottom", chatBarHeight + 4 + "px");
            }, 400);
        }, timeOut);
    }
    else {
        $("#" + element).fadeIn(0);
        $("#" + element).css("z-index", "0");
        $("#" + element).css("margin-bottom", chatBarHeight + 20 + "px");
        setTimeout(function () {
            $("#" + element).css("z-index", "1001");
            $("#" + element).css("margin-bottom", chatBarHeight + 4 + "px");
        }, 400);
    }
}
function insideBoxClose(closeAll, element) {
    $(".message-box").css("pointer-events", "auto");
    $(".messages-container-by-height").css("margin-bottom", "-400px");
    if (!closeAll) {
        $("#" + element).css("z-index", "0");
        if (parseInt($("#" + element).css("margin-bottom")) >= 24) {
            $("#" + element).css("margin-bottom", parseInt($("#" + element).css("margin-bottom")) + 20 + "px");
        }
        else $("#" + element).css("margin-bottom", "24px");
        setTimeout(function () {
            $("#" + element).css("margin-bottom", "-1200px");
            $(".messages-container-by-height").fadeOut(300);
        }, 400);
        setTimeout(function () {
            $("#" + element).fadeOut(0);
            $("#" + element).css("z-index", "1001");
        }, 750);
    }
    else {
        $(".messages-container").css("margin-bottom", "24px");
        $(".messages-container").css("z-index", "0");
        setTimeout(function () {
            $(".messages-container").css("margin-bottom", "-1200px");
            $(".messages-container-by-height").fadeOut(300);
        }, 400);
        setTimeout(function () {
            $(".messages-container").fadeOut(0);
            $(".messages-container").css("z-index", "1001");
        }, 750);
    }
}

function animatedOpen(forAll, element, sticky, closeAllOtherContainers) {
    if (fullWidth < 768) $(".btn-fixed-close-sidebar").fadeIn(300);

    if (forAll) {
        let botOffNavbarAdditionalValue = botOffNavbarH;
        if ($("#" + element).hasClass("smallside-box-container")) {
            botOffNavbarH = 0;
            sticky = false;
        }

        if (!closeAllOtherContainers) {
            $("." + element).fadeIn(200);
            if (sticky) {
                $("." + element).css("bottom", botOffNavbarH + 18 + "px");
                setTimeout(function () {
                    $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                    botOffNavbarH = botOffNavbarAdditionalValue;
                }, 600);
            }
            else {
                $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                botOffNavbarH = botOffNavbarAdditionalValue;
            }
        }
        else {
            animatedClose(true, "main-container", true, true);
            setTimeout(function () {
                $("." + element).fadeIn(200);
                if (sticky) {
                    $("." + element).css("bottom", botOffNavbarH + 18 + "px");
                    setTimeout(function () {
                        $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                    }, 600);
                }
                else {
                    $("." + element).css("bottom", botOffNavbarH + 2 + "px");
                }
            }, 300);
        }
    }
    else {
        let botOffNavbarAdditionalValue = botOffNavbarH;
        if ($("#" + element).hasClass("smallside-box-container")) {
            botOffNavbarH = 0;
            sticky = false;
        }

        if (!closeAllOtherContainers) {

            $("#" + element).fadeIn(200);
            if (sticky) {
                $("#" + element).css("bottom", botOffNavbarH + 18 + "px");
                setTimeout(function () {
                    $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                    botOffNavbarH = botOffNavbarAdditionalValue;
                }, 600);
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
                    }, 600);
                }
                else {
                    $("#" + element).css("bottom", botOffNavbarH + 2 + "px");
                }
            }, 300);
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
    else $("#Alert_Container-Text").html("No specific text for this alert");

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
            case 1:
                $("#Alert_Container-Btn1").on("click", function () {
                    document.location.href = btn1WhatToDo;
                });
                break;
            case 2:
                navigator.clipboard.writeText($("#" + btn1WhatToDo).html());
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
            case 1:
                $("#Alert_Container-Btn2").on("click", function () {
                    document.location.href = btn2WhatToDo;
                });
            case 2:
                navigator.clipboard.writeText($("#" + btn2WhatToDo).html());
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
    let allBotNavbars = document.getElementsByClassName("bot-navbar");
    for (let i = 0; i < allBotNavbars.length; i++) {
        if (parseInt($("#" + allBotNavbars[i].id).css("bottom")) >= 0) {
            botOffNavbarH = $("#" + allBotNavbars[i].id).innerHeight();
            break;
        }
    }

    if (botOffNavbarH <= 0) botOffNavbarH = $("#MainBotOffNavbar").innerHeight();
    let topNavbarH = $(".top-navbar").innerHeight();
    let neededH = fullHeight - 24 - botOffNavbarH - topNavbarH;
    let sideBarStatus = $("#Main_SideBar").css("margin-left");
    sideBarStatus = sideBarStatus == undefined ? -1200 : parseInt(sideBarStatus);

    $(".main-container").css("max-height", neededH + "px");
    $(".smallside-box-container").css("max-height", neededH + "px");
    $(".smallside-bot-navbar").css("max-height", neededH + "px");
    $(".mh-max").css("height", neededH + "px");
    $(".messages-container").css("bottom", botOffNavbarH + 12 + "px");

    if (sideBarStatus < 0) {
        $(".bot-navbar").css("width", "100%");
        $(".main-container").css("width", "100%");
        $(".main-container").css("left", 0);
        $(".postheader-container").css("left", 0);
        $(".header-container").css("left", 0);
        $(".top-navbar").css("width", "100%");
        $(".top-navbar").css("left", 0);
        $(".messages-container").css("width", fullWidth - 10 + "px");
        $(".messages-container").css("left", "5px");
    }
    else {
        if (width < 768) {
            $(".bot-navbar").css("width", "100%");
            $(".bot-navbar").css("left", 0);
            $("#Main_SideBar").css("left", "-1200px");
            $("#Main_SideBar").css("width", "100%");
            $("#Main_SideBar").fadeOut(0);

            $(".main-container").css("width", "100%");
            $(".header-container").css("width", "100%");
            $(".postheader-container").css("width", "100%");
            $(".main-container").css("left", 0);
            $(".smallside-box-container").css("width", "100%");
            $(".smallside-box-container").css("left", 0);
            $(".smallside-bot-navbar").css("left", 0);
            $(".smallside-bot-navbar").css("width", "100%");
            $(".collapse-horizontal-container").css("width", fullWidth + "px");
            $(".messages-container").css("width", fullWidth - 10 + "px");
            $(".messages-container").css("left", "5px");
        }
        else {
            $("#Main_SideBar").css("width", "35%");
            setTimeout(function () {
                let leftBarW = $("#Main_SideBar").innerWidth() + 3;
                let leftW = fullWidth - leftBarW - 3;
                let smallSideContainerW = leftBarW - 5;
                $(".bot-navbar").css("width", leftW + "px");
                $(".bot-navbar").css("width", leftW + "px");
                $(".bot-navbar").css("left", leftBarW + "px");
                $(".top-navbar").css("width", leftW + "px");
                $(".top-navbar").css("left", leftBarW + "px");

                $("#Main_SideBar").fadeIn(0);
                $("#Main_SideBar").css("left", 0);
                $(".main-container").css("width", leftW + "px");
                $(".main-container").css("left", leftBarW + "px");
                $(".smallside-box-container").css("width", smallSideContainerW + 2 + "px");
                $(".smallside-bot-navbar").css("width", smallSideContainerW + 2 + "px");
                $(".collapse-horizontal-container").css("width", smallSideContainerW * 0.975 + "px");
                $(".messages-container").css("width", leftW - 20 + "px");
                $(".messages-container").css("left", leftBarW + 10 + "px");
                $(".smallside-bot-navbar").fadeIn(0);
                $(".smallside-bot-navbar").css("bottom", 0);
            }, 350);
        }
    }
}

$("#EcoModeOnAt").on("change", function () {
    let batteryLevel = $(this).val();
    if (batteryLevel <= 100 && batteryLevel > 75) {
        $("#BatteryLevelIndicator_Span").html('<i class="fas fa-battery-full"></i> ' + batteryLevel + "%");
    }
    else if (batteryLevel <= 75 && batteryLevel > 50) {
        $("#BatteryLevelIndicator_Span").html('<i class="fas fa-battery-three-quarters"></i> ' + batteryLevel + "%");
    }
    else if (batteryLevel <= 50 && batteryLevel > 20) {
        $("#BatteryLevelIndicator_Span").html('<i class="fas fa-battery-half"></i> ' + batteryLevel + "%");
    }
    else if (batteryLevel <= 20 && batteryLevel > 5) {
        $("#BatteryLevelIndicator_Span").html('<i class="fas fa-battery-quarter text-danger"></i> ' + batteryLevel + "%");
    }
    else {
        $("#BatteryLevelIndicator_Span").html('<i class="fas fa-battery-empty text-danger"></i> ' + batteryLevel + "%");
    }
});

function getBatteryLevel(indicateTheLevel) {
    let batteryLevel = 0;
    let isCharging = false;
    if (batteryInfo) {
        navigator.getBattery()
            .then(function (battery) {
                batteryLevel = Math.floor(battery.level * 100);
                isCharging = battery.charging;

                if (indicateTheLevel) {
                    if (!isCharging) {
                        if (batteryLevel <= 100 && batteryLevel > 75) {
                            $(".battery-level-indicator").html('<span class="badge bg-dark text-light"> <i class="fas fa-battery-full"></i> ' + batteryLevel + "%</span>");
                        }
                        else if (batteryLevel <= 75 && batteryLevel > 50) {
                            $(".battery-level-indicator").html('<span class="badge bg-dark text-light"> <i class="fas fa-battery-three-quarters"></i> ' + batteryLevel + "%</span>");
                        }
                        else if (batteryLevel <= 50 && batteryLevel > 20) {
                            $(".battery-level-indicator").html('<span class="badge bg-dark text-light"> <i class="fas fa-battery-half"></i> ' + batteryLevel + "%</span>");
                        }
                        else if (batteryLevel <= 20 && batteryLevel > 5) {
                            $(".battery-level-indicator").html('<span class="badge bg-dark text-light"> <i class="fas fa-battery-quarter text-danger"></i> ' + batteryLevel + "%</span>");
                        }
                        else {
                            $(".battery-level-indicator").html('<span class="badge bg-dark text-danger"> <i class="fas fa-battery-empty text-danger"></i> ' + batteryLevel + "%</span>");
                        }
                    }
                    else {
                        $(".battery-level-indicator").html('<span class="badge bg-success text-light"> <i class="fas fa-bolt"></i> ' + batteryLevel + "%</span>");
                    }
                }
            })
            .catch(function (e) {
                console.error(e);
            });
    }
}

$(document).on("change", ".checkbox-select", function (event) {
    let value = $("#" + event.target.id).prop("checked");
    let description = $("#" + event.target.id).attr("data-bs-html");
    if (value) {
        $("#" + event.target.id).val(true);
    }
    else {
        $("#" + event.target.id).val(false);
    }

    if (description != undefined) {
        $("#" + event.target.id).attr("data-bs-html", $("#" + event.target.id + "-Description").text());
        $("#" + event.target.id + "-Description").html(description);
    }
});

$(".main-container").on("scroll", function (event) {
    let scrollH = $(this).scrollTop();
    let maxScrollH = document.getElementById(event.target.id).scrollHeight;
    if (scrollH > 0 && maxScrollH + 20 >= fullHeight) {
        headerScrollTransformation(event.target.id);
    }
    else {
        headerScrollReturn(event.target.id);
        if (currentUrl.toLowerCase().includes("/discuss")) {
            if (parseInt($("#GOM_SkipCount_Val").val()) < parseInt($("#SentMessagesCount_Val").val())) {
                $("#GetDiscussionsOlderMessages_Form").submit();
            }
        }
    }
});

$("#YTMiniPlayer_Search").on("change", function () {
    ytVideoOptimizer($(this).val());
});

$(document).on("mouseover", ".info-popover", function () {
    $(this).popover("show");
});
$(document).on("mouseout", ".info-popover", function () {
    $(this).popover("hide");
});

//$("#GetDiscussionMessageReactions_Form").on("submit", function (event) {
//    event.preventDefault();
//    let url = $(this).attr("action");
//    let data = $(this).serialize();

//    $.get(url, data, function (response) {
//        if (response.success) {
//            verySmallBoxClose(response.id + "-Reactions_Container");
//            if (response.count > 0) {
//                setTimeout(function () {
//                    $("#" + response.id + "-Reactions_Container").empty();
//                }, 400);
//                setTimeout(function () {
//                    $.each(response.result, function (index) {
//                        let reactionBubble = $("<span class='reaction-bubble'></span>");
//                        reactionBubble.html(response.result[index][0].reactionCode + " " + response.result[index].length);
//                        $("#" + response.id + "-Reactions_Container").append(reactionBubble);
//                    });
//                }, 400);
//            }
//            setTimeout(function () {
//                verySmallBoxOpen(response.id + "-Reactions_Container");
//            }, 400);
//        }
//        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
//    });
//});
//$("#SetDiscussionMessageReaction_Form").on("submit", function (event) {
//    event.preventDefault();
//    let url = $(this).attr("action");
//    let data = $(this).serialize();

//    $.post(url, data, function (response) {
//        if (response.success) {
//            if (response.reactionsCount > 0) {
//                $("#" + response.result + "-Reactions_Container").empty();
//                let countLbl = $("<span class='card-text get-reactions'></span>");
//                countLbl.attr("id", response.result + "-Reactions_Lbl");
//                countLbl.html(" <i class='fa-solid fa-icons'></i> " + response.reactionsCount + " reactions");
//                $("#" + response.result + "-Reactions_Container").append(countLbl);

//                verySmallBoxOpen(response.result + "-Reactions_Container");
//            }
//            else {
//                $("#" + response.result + "-Reactions_Container").empty();
//                $("#" + response.result + "-Reactions_Container").slideUp(350);
//            }
//        }
//        else alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Close", null, 0, null, null, null, 3.5);
//    });
//});

//$(document).on("click", ".get-reactions", function (event) {
//    let trueId = getTrueId(event.target.id);
//    if (trueId != null) {
//        $("#GDMR_Id_Val").val(trueId);
//        $("#GetDiscussionMessageReactions_Form").submit();
//    }
//    else $("#GDMR_Id_Val").val(0);
//});
//$(document).on("click", ".set-reaction", function (event) {
//    let trueId = getTrueId(event.target.id);
//    if (trueId != null) {
//        $("#SDMR_ReactionId_Val").val(trueId);
//        $("#SetDiscussionMessageReaction_Form").submit();
//    }
//    else $("#SDMR_ReactionId_Val").val(0);
//});    //SendDiscussionMessage_SbmtBtn