﻿let fullWidth = 0;
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

    if ($("#AvatarSticker_Hdn_Val").val() != "") {
        $(".unpictured-container-label").html($("#AvatarSticker_Hdn_Val").val());
        $(".unpictured-container-label-sm").html($("#AvatarSticker_Hdn_Val").val());
    }

    insideBoxOpen("PreloadedInside_Box", true);
    statusSlider("StatusBar_Lbl", 1);
    replaceAllTheTextInMessages();

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

$("#GetDiscussions_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.get(url, data, function (response) {
        $("#Discussions_Container-Box").empty();
        if (response.success) {
            $("#Discussions_Container_Header").text("Discussions");
            if (response.count == 1) $("#DiscussionsCount_Span").text("single");
            else $("#DiscussionsCount_Span").text(response.count);
            if (response.count > 0) {
                let tooltipReminder = $("<div class='box-container bg-light p-1'></div>");
                let tolltipReminderText = $("<span class='h6'>Tap on discussion's name to open it</span>");
                let tooltipDivider = $("<div></div>");
                let tooltipSmallReminder = $("<small class='card-text text-muted'>Tap on others icon to edit other settings</small>");
                tooltipReminder.append(tolltipReminderText);
                tooltipReminder.append(tooltipDivider);
                tooltipReminder.append(tooltipSmallReminder);
                $("#Discussions_Container-Box").append(tooltipReminder);

                $.each(response.result, function (index) {
                    let div = $("<div class='box-container bg-light p-2 mt-2'></div>");
                    let name = $("<h6 class='h6 relocate-to-discussion'></h6>");
                    let joinedAndCreated = $("<small class='card-text text-muted relocate-to-discussion'></small>");
                    let isMutedIcon = $('<small class="card-text text-orange float-end ms-2 mt-1"> <i class="fa-regular fa-bell-slash"></i> </small>');
                    let isPinnedIcon = $("<small class='card-text float-end ms-2 mt-1'> <i class='fa-solid fa-thumbtack'></i> </small>");

                    let dropdownDiv = $("<div class='dropdown'></div>");
                    let dropdownBtn = $("<button type='button' class='btn btn-standard btn-sm float-end ms-2' data-bs-toggle='dropdown' aria-expanded='false'> <i class='fa-solid fa-ellipsis-h'></i> </button>");
                    let dropdownUl = $("<ul class='dropdown-menu p-1'></ul>");
                    let dropdownLi0 = $("<li class='text-center'></li>");
                    let dropdownLi1 = $("<li></li>");
                    let dropdownLi2 = $("<li></li>");
                    let dropdownLi3 = $("<li></li>");
                    let dropdownHeader = $("<small class='card-text text-muted fw-500'></small>");
                    let dropdownBtn1 = $("<a href='#' class='dropdown-item mt-2'> <i class='fa-solid fa-up-right-from-square'></i> Discussion</a>");
                    let dropdownBtn2 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    let dropdownBtn3 = $("<button type='button' class='dropdown-item mt-1'></button>");
                    dropdownLi0.append(dropdownHeader);
                    dropdownLi1.append(dropdownBtn1);
                    dropdownLi2.append(dropdownBtn2);
                    dropdownLi3.append(dropdownBtn3);
                    dropdownUl.append(dropdownLi0);
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
                    joinedAndCreated.html("created " + dateAndTimeTranslator(response.result[index].createdAt) + ", joined " + dateAndTimeTranslator(response.result[index].joinedAt));
                    dropdownHeader.text(dateAndTimeTranslator(response.result[index].createdAt));
                    dropdownBtn1.attr("href", "/Discussion/Discuss/" + response.result[index].discussionId);
                    dropdownBtn2.attr("id", response.result[index].discussionId + "-MuteOrUnmuteTheDiscussion");
                    dropdownBtn3.attr("id", response.result[index].discussionId + "-PinOrUnpinTheDiscussion");

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

                    div.append(dropdownDiv);
                    div.append(isPinnedIcon);
                    div.append(isMutedIcon);
                    div.append(name);
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
                    let userName = $("<span class='h6 get-user-info-by-id'></span>");
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
                    dropdownBtn1.attr("href", "/User/Info/" + response.result[index].userId);
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
            alert('<i class="fa-solid fa-plus fa-spin text-primary" style="--fa-animation-duration: 1s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Done", null, 0, null, null, null, 4.25);
            slideToLeft("MembersInfoMain_Container");
            $("#GetMembersInfo_SbmtBtn").attr("disabled", false);
        }
        else {
            if (parseInt(response.error) == -256) alert('<i class="fa-solid fa-user-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
            else if (parseInt(response.error) == -128) alert('<i class="fa-solid fa-user-xmark"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
            else alert('<i class="fa-regular fa-circle-xmark fa-shake text-warning" style="--fa-animation-duration: 2.3s; --fa-animation-iteration-count: 1;"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});

$("#JoinToDiscussion_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            insideBoxClose(false, "SendMessage_Box");
            setTimeout(function () {
                setTimeout(function () {
                    $("#SendMessage_SecondInside_Box").fadeOut(300);
                    $("#SendMessage_FirstInside_Box").fadeIn(300);
                });
            }, 450);
            setTimeout(function () {
                insideBoxClose(false, "PreloadedInside_Box");
            }, 600);
            setTimeout(function () {
                insideBoxOpen("SendMessage_Box", false);
            }, 900);
            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> <br/>Joined');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", true);
            $("#SendDiscussionMessage_Text_Val").attr("readonly", false);
            $("#SendDiscussionMessage_Text_Val").attr("disabled", false);
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
            insideBoxClose(false, "SendMessage_Box");
            setTimeout(function () {
                setTimeout(function () {
                    $("#SendMessage_SecondInside_Box").fadeOut(300);
                    $("#SendMessage_FirstInside_Box").fadeIn(300);
                });
            }, 450);
            setTimeout(function () {
                insideBoxClose(false, "PreloadedInside_Box");
            }, 600);
            setTimeout(function () {
                insideBoxOpen("SendMessage_Box", false);
            }, 900);
            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-regular fa-circle-check"></i> <br/>Joined');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", true);
            $("#SendDiscussionMessage_Text_Val").attr("readonly", false);
            $("#SendDiscussionMessage_Text_Val").attr("disabled", false);
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
            insideBoxClose(false, "SendMessage_Box");
            setTimeout(function () {
                $("#SendMessage_SecondInside_Box").fadeIn(300);
                $("#SendMessage_FirstInside_Box").fadeOut(300);
            }, 450);
            setTimeout(function () {
                insideBoxOpen("SendMessage_Box", false);
            }, 750);
            setTimeout(function () {
                insideBoxOpen("PreloadedInside_Box", true);
            }, 1200);

            $("#JoinToDiscussion_SbmtBtn").html(' <i class="fa-solid fa-right-to-bracket"></i> <br/>Join');
            $("#JoinToDiscussion_SbmtBtn").attr("disabled", false);
            $("#SendDiscussionMessage_Text_Val").attr("readonly", true);
            $("#SendDiscussionMessage_Text_Val").attr("disabled", true);
            $("#SendDiscussionMessage_SbmtBtn").attr("disabled", true);
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

$("#SendDiscussionMessage_Form").on("submit", function (event) {
    let finalCut = replaceAllUsersInText($("#SendDiscussionMessage_Text_Val").val());
    let linksCut = replaceAllLinksInText($("#SendDiscussionMessage_Text_Val").val());
    if (finalCut != null) {
        $("#SendDiscussionMessage_Text_Val").val(finalCut);
        if (linksCut != null) $("#SendDiscussionMessage_Text_Val").val(linksCut);
    }
    event.preventDefault();

    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#SendDiscussionMessage_Text_Val").val(null);
            $("#SendDiscussionMessage_SbmtBtn").attr("disabled", true);

            let sentMessagesCount = $("#SentMessagesCount_Val").val();

            let nextMsgSeparator = $("<div><br/><br/><br/><br/></div>");
            let mainMessageBox = $("<div class='current-user-message-main-box float-end p-2'></div>");
            let textBox = $("<div class='current-user-message-box p-2 mt-1'></div>");
            let textElement = $("<p class='card-text white-space-on'></p>");
            let additionalInfoBox = $("<div class='mt-1 me-1 float-end'></div>");
            let additionalInfoSmall = $("<small class='card-text text-muted'></small>");
            let isCheckedSpan = $("<span></span>");
            let dateTimeSpan = $("<span></span>");

            textElement.html(response.result.text);
            isCheckedSpan.html(' <i class="fa-solid fa-check"></i> ');
            dateTimeSpan.text(dateAndTimeTranslator(response.result.sentAt));
            additionalInfoSmall.append(isCheckedSpan);
            additionalInfoSmall.append(dateTimeSpan);
            textBox.append(textElement);
            additionalInfoBox.append(additionalInfoSmall);
            mainMessageBox.append(textElement);
            mainMessageBox.append(additionalInfoBox);

            if (sentMessagesCount <= 0) {
                $("#Presentation_Box").fadeOut(350);
                $("#NoSentMessages_Box").fadeOut(350);
                setTimeout(function () {
                    $("#Messages_Container_Box").append(mainMessageBox);
                    $("#Messages_Container_Box").append(nextMsgSeparator);
                }, 400);
            }
            else {
                $("#Messages_Container_Box").append(mainMessageBox);
                $("#Messages_Container_Box").append(nextMsgSeparator);
            }
            $("#SentMessagesCount_Val").val(++sentMessagesCount);
            $("#DiscussionStatusBar_Lbl").text(sentMessagesCount + " messages");
            setTimeout(function () {
                $("#SendDiscussionMessage_SbmtBtn").attr("disabled", false);
            }, 800);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
});

$("#SendDiscussionReply_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            $("#SendDiscussionReply_SbmtBtn").attr("disabled", true);
            $("#SR_Text_Val").val("");
            $("#SR_MessageId_Val").val(0);
            insideBoxClose(false, "ReplyToDiscussionMessage_Box");
            setTimeout(function () {
                insideBoxOpen("SendMessage_Box", false);
            }, 750);
            setTimeout(function () {
                $("#SendDiscussionReply_SbmtBtn").attr("disabled", false);
            }, 1800);
        }
        else {
            $("#SR_Text_Val").val("");
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4);
        }
    });
});

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

            if (response.userId == response.result.userId) {
                $("#EditDiscussionMsg_Col").fadeIn(0);
                $("#DeleteDiscussionMsg_Col").fadeIn(0);
                $("#DDM_Id_Val").val(response.id);

                if (response.result.daysPassed > 4) {
                    $("#EditDiscussionMessage_Box-Open").attr("disabled", true);
                }
                else {
                    $("#EditDiscussionMessage_Box-Open").attr("disabled", false);
                }
            }
            else {
                $("#EditDiscussionMsg_Col").fadeOut(0);
                $("#DeleteDiscussionMsg_Col").fadeOut(0);
            }
            $("#SR_MessageId_Val").val(response.id);
            $("#SR_MessageText_Val").val(response.result.text.substring(0, 40));
            $(".messages-container").css("margin-bottom", "-1200px");
            $(".messages-container").fadeOut(100);
            insideBoxOpen("DiscussionOptions_Box");

            setTimeout(function () {
                $("#DiscussionOptionMessageText_Lbl").html(response.result.text);
                $("#EditMessage_Text_Lbl").html(response.result.text);
                $("#ReplyingMessage_Text_Lbl").html(response.result.text);
                textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "DiscussionOptionMessageText_Lbl");
                textDecoder($("#DiscussionOptionMessageText_Lbl").html(), "EditMessage_Text_Lbl");
                $("#EM_NewText_Val").val(response.result.text);
                $("#DiscussionOptionAdditionalInfo_Lbl").html(isChecked + " " + dateAndTimeTranslator(response.result.sentAt) + isEdited);
                $("#CurrentGotDiscussionMsg_Id_Val").val(response.id);
                $("#EM_Id_Val").val(response.id);
            }, 150);
        }
        else {
            alert('<i class="fa-regular fa-circle-xmark text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.25);
        }
    });
});
$("#EditMessage_Form").on("submit", function (event) {
    event.preventDefault();
    let url = $(this).attr("action");
    let data = $(this).serialize();

    $.post(url, data, function (response) {
        if (response.success) {
            insideBoxClose(false, "EditDiscussionMessage_Box");
            insideBoxOpen("SendMessage_Box");
            $("#" + response.id + "-DiscussionMessage_IsEdited").fadeIn(350);
            $("#" + response.id + "-DiscussionOptionMsgText_Lbl").html(response.text);
            $("#" + response.id + "-DiscussionMsgReplyText").html(response.text);
        }
        else {
            alert('<i class="fa-solid fa-ban text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
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
                }, 400);
            }
            else {
                $("#StatusBar_Lbl-1").text(currentCount + " sent messages");
                $("#" + response.id + "-DiscussionMsgBox").fadeOut(350);
                $("#" + response.id + "-DiscussionMsgReplyText").text("Deleted message");
            }
            insideBoxClose(false, "DiscussionOptions_Box");
            insideBoxOpen("SendMessage_Box");
            $("#SentMessagesCount_Val").val(currentCount);
        }
        else {
            alert('<i class="fa-solid fa-ban text-warning"></i>', response.alert, "Got It", null, 0, null, null, null, 4.5);
        }
    });
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
                let header = $('<small class="card-text text-muted"> <i class="fa-solid fa-magnifying-glass"></i> Find new members to add them to this discussion</small>');

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

$("#ScheduleTheMessage_Btn").on("click", function () {
    let text = $("#SendDiscussionMessage_Text_Val").val();
    let userId = $("#SendMessage_UserId_Val").val();
    let discussionId = $("#SendMessage_DiscussionId_Val").val();
    let isAutoDeletable = $("#SendMessage_IsAutoDeletable").val();

    $("#ScheduledMsg_Text").html(text);
    $("#ScheduledMsg_IsAutoDeletable").text(isAutoDeletable == null ? "Non-autodeletable" : "Autodeletable");

    animatedOpen(false, "ScheduleTheMessage_Container", true, true);
});

$("#ScheduledMessage_AddDays").on("change", function () {
    let currentDate = new Date();
    let addedDays = parseInt($("#ScheduledMessage_AddDays").val());
    let addedMinutes = parseInt($("#ScheduledMessage_AddDays").val());
    let addedHours = parseInt($("#ScheduledMessage_AddHours").val());

    addedMinutes += addedDays * 24 * 60;
    let neededMilliseconds = addedMinutes * 60000;
/*    addedMinutes += addedHours * 60;*/
    console.log(neededMilliseconds);
    let newDate = new Date(currentDate);
    newDate.setDate(currentDate.getTime() + neededMilliseconds);
    console.log(newDate.getDate());
    //console.log(newDate.getDate() + "/" + parseInt(newDate.getMonth()) + "/" + newDate.getFullYear() + ", " + newDate.getHours() + ":" + newDate.getMinutes());
});
$("#ScheduledMessage_AddHours").on("change", function () {
    let currentDate = new Date();
    let addedDays = $("#ScheduledMessage_AddDays").val();
    let addedMinutes = $("#ScheduledMessage_AddDays").val();
    let addedHours = $("#ScheduledMessage_AddHours").val();

    addedMinutes += addedDays * 24 * 60;
    addedMinutes += addedHours * 60;

    let newDate = new Date(currentDate);
    newDate.setDate(currentDate.getMinutes() + addedMinutes);
    console.log(newDate.getDate());
});
$("#ScheduledMessage_AddMinutes").on("change", function () {
    let currentDate = new Date();
    let addedDays = $("#ScheduledMessage_AddDays").val();
    let addedMinutes = $("#ScheduledMessage_AddDays").val();
    let addedHours = $("#ScheduledMessage_AddHours").val();

    addedMinutes += addedDays * 24 * 60;
    addedMinutes += addedHours * 60;

    let newDate = new Date(currentDate);
    newDate.setDate(currentDate.getMinutes() + addedMinutes);
    console.log(newDate.getDate());
});

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

    if (trueId != null && trueId > 0) {
        $("#SR_MessageId_Val").val(trueId);
        $("#SR_MessageText_Val").val($("#" + trueId + "-ReplyToDiscussionMessage").html().substring(0, 40));
        $("#ReplyingMessage_Text_Lbl").text($("#" + trueId + "-ReplyToDiscussionMessage").html().substring(0, 40));
        insideBoxClose(true, "messages-container");
        setTimeout(function () {
            insideBoxOpen("ReplyToDiscussionMessage_Box", false);
        }, 750);
    }
    else {
        $("#SR_MessageId_Val").val(0);
        $("#SR_MessageText_Val").val(null);
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
})
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

$(document).on("click", ".delete-discussion-message", function (event) {
    let id = $("#DDM_Id_Val").val();
    if (id > 0) {
        $("#DeleteDiscussionMessage_Form").submit();
    }
    else $("#DDM_Id_Val").val(0);
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

$(document).on("click", ".auto-delete-on", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $(this).addClass("auto-delete-off");
        $(this).removeClass("auto-delete-on");
        $(this).html(' <i class="fa-regular fa-circle-check text-primary"></i> Auto-Delete Enabled');
        $("#" + trueId).val(true);
    }
    else $("#" + trueId).val(false);
});
$(document).on("click", ".auto-delete-off", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#" + trueId).val(false);
        $(this).html(' <i class="fa-solid fa-clock-rotate-left"></i> Auto-Deletable Message');
        $(this).addClass("auto-delete-on");
        $(this).removeClass("auto-delete-off");
    }
    else $("#" + trueId).val(false);
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
        $(".messages-container").css("margin-bottom", "-1200px");
        $(".messages-container").fadeOut(300);
        setTimeout(function () {
            insideBoxOpen(trueId);
        }, 150);
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
            $("#SendDiscussionMessage_Text_Val").val(command);
            $("#SendLiveDiscussionMessage_Text_Val").val(command);
            $("#SendChatMessage_Text_Val").val(command);

            $("#SendDiscussionMessage_Text_Val").change();
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
        setTimeout(function () {
            insideBoxOpen("SendMessage_Box");
        }, 750);
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

$(document).on("click", ".delete-message-options", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        insideBoxClose(false, trueId);
        insideBoxOpen("SendMessage_Box");
        insideBoxOpen("SendLiveDiscussionMessage_Box");
        insideBoxOpen("SendChatMessage_Box");
        setTimeout(function () {
            $(".got-msg-text").html("Message Text");
            $(".got-msg-datetime").html("sent date, time, is checked and edited");
            $(".got-msg-id-value").val(0);
        }, 450);
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
        $("#" + trueId).val(setBackAllUsersInText($("#" + trueId).val()));
        $("#" + trueId).val(replaceAllUsersInText($("#" + trueId).val()));
        //$("#" + trueId).val(setBackAllUsersInText($("#" + trueId).val()));
        $("#" + trueId).val(replaceAllLinksInText($("#" + trueId).val()));
        console.log(replaceAllLinksInText($("#" + trueId).val()));
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

$(document).on("click", ".btn-file-click", function (event) {
    let trueId = getTrueId(event.target.id);
    if (trueId != null) {
        $("#" + trueId).click();
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
        headerReturn(trueId);
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
    if (value[0] == "/") $("#SendDiscussionMessage_SbmtBtn").attr("disabled", true);
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
                    insideBoxOpen("CommandLine_Box", true);
                }, 750);
            }
            else {
                insideBoxOpen("CommandLine_Box", true);
            }
        }
    }
});

function getTrueId(name) {
    let trueId = name.substring(0, name.lastIndexOf("-"));
    if (trueId != null || trueId != undefined) return trueId;
    else return null;
}

function openSidebar() {
    if (fullWidth < 768) {
        $(".btn-fixed-close-sidebar").fadeIn(300);
        $("#Main_SideBar").fadeIn(250);
        $("#Main_SideBar").css("left", 0);
    }
}

function closeSidebar() {
    if (fullWidth < 768) {
        animatedClose(true, "smallside-box-container ", true, true);
        $(".btn-fixed-close-sidebar").fadeOut(300);
        setTimeout(function () {
            $("#Main_SideBar").fadeOut(350);
            $("#Main_SideBar").css("left", "-1200px");
        }, 550);
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
    $(".header-container").slideUp(300);
    $(".discussion-box").slideUp(300);
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
        $(".header-container").fadeIn(300);
        $(".discussion-box").fadeIn(300);
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
                fullText = part1 + "[/" + selectedText + "]] " + part2;
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

function statusSlider(mainId, maxCount) {
    if (maxCount > 0) {
        let currentCount = 0;
        setInterval(function () {
            $(".status-slider").fadeOut(350);
            setTimeout(function () {
                $("#" + mainId + "-" + currentCount).fadeIn(350);
            }, 300);
            currentCount++;
            if (currentCount > maxCount) currentCount = 0;
        }, 4500);
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
    text = text.replaceAll("[*", "<span class='user-via-shortname'>");
    text = text.replaceAll("!]", "</button>");
    text = text.replaceAll("$!", "</a>");

    $("#" + setIn).html(text);
}

function replaceAllTheTextInMessages() {
    let allTheText = document.getElementsByClassName("message-label");
    if (allTheText.length > 0) {
        for (let i = 0; i < allTheText.length; i++) {
            textDecoder(document.getElementById(allTheText[i].id).innerHTML, allTheText[i].id);
        }
    }
}

function setBackAllUsersInText(text) {
    text = text.replaceAll("[*", "");
    text = text.replaceAll("*]", "");

    return text;
}

function replaceAllLinksInText(text) {
    let linksArray = [];
    let currentIndex = 0;

    for (let i = 0; i < text.length; i++) {
        if (text.substring(currentIndex, text.length).includes("https")) {
            currentIndex = text.indexOf("https", currentIndex);
            let endIndex = text.indexOf(" ", currentIndex);
            let link = text.substring(currentIndex, endIndex);
            console.log(link);

            linksArray.push(link);
        }
    }

    if (linksArray.length > 0) return linksArray;
    else return null;
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
        $("#FirstReserve_Btn").html(' <i class="fa-solid fa-bars text-muted"></i> <br/><small>Menu</small>');
        $("#FirstReserve_Btn").addClass("open-sidebar");
        $("#FirstMain_Btn").fadeOut(0);
        $("#FirstReserve_Btn").fadeIn(0);
    }
}

function slide(forAll, element) {
    //if (!forAll) {
    //    $(".slider-container").css("margin-left", "32px");
    //    setTimeout(function () {
    //        $(".slider-container").css("margin-left", "-6000px");
    //        $(".slider-container").fadeOut(350);
    //    }, 350);
    //    setTimeout(function () {
    //        $("#" + element).fadeIn(0);
    //    }, 700);
    //    setTimeout(function () {
    //        $("#" + element).css("margin-left", "40px");
    //    }, 750);
    //    setTimeout(function () {
    //        $("#" + element).css("margin-left", 0);
    //    }, 1200);
    //}

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

function insideBoxOpen(element, doNotCloseChatBox) {
    $("#" + element).fadeIn(0);
    $("#" + element).css("z-index", "0");
    setTimeout(function () {
        if (!doNotCloseChatBox) $("#" + element).css("margin-bottom", "8px");
        else {
            let chatBoxHeight = $("#SendMessage_Box").innerHeight();
            $("#" + element).css("margin-bottom", parseInt(chatBoxHeight) + 4 + "px");
        }
        setTimeout(function () {
            $("#" + element).css("z-index", "1001");
        }, 225);
    }, 350);
}
function insideBoxClose(closeAll, element) {
    if (!closeAll) {
        $("#" + element).css("z-index", "0");
        if (parseInt($("#" + element).css("margin-bottom")) >= 24) {
            $("#" + element).css("margin-bottom", parseInt($("#" + element).css("margin-bottom")) + 20 + "px");
        }
        else $("#" + element).css("margin-bottom", "24px");
        setTimeout(function () {
            $("#" + element).css("margin-bottom", "-1200px");
        }, 400);
        setTimeout(function () {
            $("#" + element).fadeOut(300);
            $("#" + element).css("z-index", "1001");
        }, 750);
    }
    else {
        $(".messages-container").css("margin-bottom", "24px");
        $(".messages-container").css("z-index", "0");
        setTimeout(function () {
            $(".messages-container").css("margin-bottom", "-1200px");
        }, 400);
        setTimeout(function () {
            $(".messages-container").fadeOut(300);
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
            case 1:
                $("#Alert_Container-Btn1").on("click", function () {
                    document.location.href = btn1WhatToDo;
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
            case 1:
                $("#Alert_Container-Btn2").on("click", function () {
                    document.location.href = btn2WhatToDo;
                });
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
    let topNavbarH = $(".top-navbar").innerHeight();
    let neededH = fullHeight - 24 - botOffNavbarH - topNavbarH;
    let sideBarStatus = $("#Main_SideBar").css("margin-left");
    sideBarStatus = sideBarStatus == undefined ? -1200 : parseInt(sideBarStatus);

    $(".main-container").css("max-height", neededH + "px");
    $(".smallside-box-container").css("max-height", neededH + "px");
    $(".mh-max").css("height", neededH + "px");
    $(".messages-container").css("bottom", botOffNavbarH + 12 + "px");

    if (sideBarStatus < 0) {
        $("#MainBotOffNavbar").css("width", "100%");
        $(".main-container").css("width", "100%");
        $(".main-container").css("left", 0);
        $(".top-navbar").css("width", "100%");
        $(".top-navbar").css("left", 0);
        $(".messages-container").css("width", fullWidth - 10 + "px");
        $(".messages-container").css("left", "5px");
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
            $(".collapse-horizontal-container").css("width", fullWidth + "px");
            $(".messages-container").css("width", fullWidth - 10 + "px");
            $(".messages-container").css("left", "5px");
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
            $(".top-navbar").css("width", leftW + "px");
            $(".top-navbar").css("left", leftBarW + "px");

            $("#Main_SideBar").fadeIn(0);
            $("#Main_SideBar").css("left", 0);
            $(".main-container").css("width", leftW + "px");
            $(".main-container").css("left", leftBarW + "px");
            $(".smallside-box-container").css("width", smallSideContainerW + 2 + "px");
            $(".collapse-horizontal-container").css("width", smallSideContainerW * 0.975 + "px");
            $(".messages-container").css("width", leftW - 20 + "px");
            $(".messages-container").css("left", leftBarW + 10  + "px");
            //$(".smallside-modal-container").css("left", "10px");
            //$(".smallside-modal-container").css("width", smallSideContainerW - 19 + "px");
        }
    }
}

$(document).on("change", ".checkbox-select", function (event) {
    let value = $("#" + event.target.id).prop("checked");
    if (value) {
        $("#" + event.target.id).val(true);
    }
    else {
        $("#" + event.target.id).val(false);
    }
});

$(document).on("mouseover", ".info-popover", function () {
    $(this).popover("show");
});
$(document).on("mouseout", ".info-popover", function () {
    $(this).popover("hide");
});