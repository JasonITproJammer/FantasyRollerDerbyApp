$(document).ready(function () {
    //SignalR events
    $.connection.hub.logging = false;
    $.connection.hub.qs = "vars=" + $("#leagueName").val() + "|" + countDownDate;
    var hub = $.connection.draft;

    //Triggers that signal to the DraftHub class to call the hub.client functions
    $.connection.hub.start().done(function () {
        SetNextRankedSkater();
        $("#triggerSignalR").click(function () {
            hub.server.draft($("#leagueName").val(), $("#skaterID").val())
        });
    });

    //function that is called by the DraftHub class to draft skater
    hub.client.draftSkater = function (skaterID) {
        console.log("draftSkater skaterID: " + skaterID);
        $("#tblSkaters tbody tr").removeClass("skaterChosen");
        $("#row" + skaterID).addClass("skaterChosen");
        $("#rowNum").val(skaterID);
        $("#skaterID").val(skaterID);

        //Scroll to selected skater
        var container = $("#divSkatersSelect");
        if ($("#scrollToCount").val() == "0") {
            var scrollTo = $("#tblSkatersHeaderRow");
            console.log("ScrollTo: " + scrollTo);
        } else {
            var scrollTo = $("#row" + skaterID);
            console.log("ScrollTo: " + scrollTo);
        }
        container.animate({ scrollTop: scrollTo.offset().top - container.offset().top + container.scrollTop() });

        //increment
        var count = $("#scrollToCount").val();
        var intCount = parseInt(count);
        intCount = intCount + 1;
        $("#scrollToCount").val(intCount);
    }

    //Set all fields to read only
    $(".disableMe").prop("disabled", true);

    //-----KICK OFF-------------------
    StartDraft();
});

//-----PUBLIC VARIABLS----------
// Set the date we're counting down to
//https://www.w3schools.com/js/js_date_methods.asp
var firstLogin = true;
var draftOrderIDs = $("#draftOrderIDs").val().split('|');
var draftOrderNames = $("#draftOrderTeamNames").val().split('|');
var draftOrderGeneric = $("#draftOrderGeneric").val().split('|');
var draftOrderTeamLogos = $("#draftOrderTeamLogos").val().split('|');
var currentDraftOrderInt = draftOrderIDs.indexOf($("#currentPickTeamID").val().toString());
var currentTeamPickingName = draftOrderNames[currentDraftOrderInt];
var currentTurnNum = 0;
var pickCounter = 0;
var totalTeams = $("#totalTeams").val();
var totalPicks = $("#totalPicks").val();
var totalRounds = $("#totalRounds").val();
var totalDraftTime = $("#totalDraftTime").val();
var timePerPick = $("#timePerPick").val();
var countDownDate = new Date();
var draftClock = new Date();
var teamClock = new Date();
var draftStartDateTime = new Date($("#draftDateTime").val());
countDownDate.setFullYear(draftStartDateTime.getFullYear(), draftStartDateTime.getMonth(), draftStartDateTime.getDate());
countDownDate.setHours(draftStartDateTime.getHours());
countDownDate.setMinutes(draftStartDateTime.getMinutes());
countDownDate.setSeconds(draftStartDateTime.getSeconds());
//console.log("totalDraftTime: " + totalDraftTime);
countDownDate = addMinutes(countDownDate, totalDraftTime);
countDownDate = addSeconds(countDownDate, timePerPick);
//console.log("countDownDate: " + countDownDate);
var nowPick = new Date().getTime();
var pickTime = new Date();

//----METHODS------------
function StartDraft() {
    $("#TeamDraftingNow").text(currentTeamPickingName);
    startDraftClock();
    KickOffPickClock();
}

// Update the count down every 1 second
function startDraftClock() {
    x = setInterval(function () {
        // Get todays date and time
        var now = new Date().getTime();

        // Find the distance between now an the count down date
        var distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds
        //var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Display the result in the element with id="demo"
        document.getElementById("draftTimer").innerHTML = hours + "h " + minutes + "m " + seconds + "s ";

        // If the count down is finished, write some text
        if (distance < 0) {
            clearInterval(x);
            //draft has finished
            console.log("---Draft has concluded---");
            document.getElementById("TeamDraftTime").innerHTML = "Draft has concluded.";
            $(".DraftCompleted").addClass("hidden");
            $(".DraftCompletedMessage").removeClass("hidden");
        }
    }, 1000);
}

function KickOffPickClock() {
    console.log("---KickOffPickClock---");
    $("#scrollToCount").val("0");
    //set pickCounter
    nowPick = new Date().getTime();
    //console.log("nowPick: " + nowPick);
    //console.log("draftStartDateTime.getTime(): " + draftStartDateTime.getTime());
    var diffStartNow = nowPick - draftStartDateTime.getTime();
    //console.log("diffStartNow: " + diffStartNow);
    var diffStartNowHours = Math.floor((diffStartNow % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    //console.log("diffStartNowHours: " + diffStartNowHours);
    var diffStartNowMinutes = Math.floor((diffStartNow % (1000 * 60 * 60)) / (1000 * 60));
    //console.log("diffStartNowMinutes: " + diffStartNowMinutes);
    var diffStartNowSeconds = Math.floor((diffStartNow % (1000 * 60)) / 1000);
    //console.log("diffStartNowSeconds: " + diffStartNowSeconds);
    diffStartNow = diffStartNowSeconds + (diffStartNowMinutes * 60) + ((diffStartNowHours * 60) * 60);
    //console.log("diffStartNow: " + diffStartNow);
    //console.log("timePerPick: " + timePerPick);
    pickCounter = diffStartNow / timePerPick;
    pickCounter = Math.floor(pickCounter);
    //console.log("pickCounter: " + pickCounter);

    //If user logs in before the draft (only allowed to log in 30 mins before)
    //Set the draft clock to a countdown to darft start clock
    var now = new Date();
    if (now < draftStartDateTime) {
        //User logged in before draft began
        firstLogin = false;
        console.log("---User logged in before draft began---");
        currentTeamPickingName = draftOrderNames[0];
        $("#draftFantasyTeamLogo").attr("src", "/Images/TeamLogos/" + draftOrderTeamLogos[0]);
        document.getElementById("TeamDraftTime").innerHTML = "Time until draft starts:";
        pickTime = draftStartDateTime;
        //console.log("pickTime = draftStartDateTime: " + pickTime);

        //Update who's on the clock
        //console.log("currentTeamPickingName: " + currentTeamPickingName);
        $("#TeamDraftingMeow").text(currentTeamPickingName);
        $("#trDraftOrderNum1").addClass("OnTheClock");

        //Allow user who's pick it is to click the draft button
        currentTeamPickingID = draftOrderIDs[0];
        //console.log("currentTeamPickingID: " + currentTeamPickingID);
        //console.log("$(#userTeamID).val(): " + $("#userTeamID").val());
        if ($("#userTeamID").val() == currentTeamPickingID) {
            $(".disableMe").prop("disabled", false);
        } else {
            $(".disableMe").prop("disabled", true);
        }

        //console.log("pickTime: " + pickTime);
        StartPickClock();
    }
    else if (now > countDownDate) {
        //User logged in after draft was completed
        console.log("---User logged in after draft was completed---");
        document.getElementById("TeamDraftTime").innerHTML = "Draft has completed.";
        $(".DraftCompleted").addClass("hidden");
    }
    else {
        //User logged in during an active draft
        console.log("---User logged in during an active draft.---");
        document.getElementById("TeamDraftTime").innerHTML = "Time Left To Pick:";
        //console.log("timePerPick: " + timePerPick);
        //console.log("totalPicks: " + totalPicks);
        //console.log("pickCounter: " + pickCounter);

        var draftTimeLeft = timePerPick * (totalPicks - pickCounter + 1);
        //console.log("draftTimeLeft: " + draftTimeLeft);
        draftTimeLeft = (draftTimeLeft - timePerPick);
        //console.log("draftTimeLeft = (draftTimeLeft - timePerPick): " + draftTimeLeft);
        //console.log("pickTime: " + pickTime);

        //pickTime = addSeconds(countDownDate, draftTimeLeft * -1);
        //console.log("pickTime = addMinutes(countDownDate, draftTimeLeft * -1): " + pickTime);

        //pickTime = addSeconds(pickTime, timePerPick);
        //console.log("countDownDate: " + countDownDate);
        //console.log("pickTime = addSeconds(pickTime, timePerPick): " + pickTime);

        //draftStartDateTime (pickCounter + 1) * timePerPick
        pickTime = addSeconds(draftStartDateTime, (pickCounter + 1) * timePerPick);
        //console.log("pickTime = addSeconds(draftStartDateTime, (pickCounter + 1) * timePerPick): " + pickTime);

        //Update who's on the clock
        //Only run when the user FIRST logs in
        if (firstLogin) {
            console.log("-----First Time Loggin In![IN]-----");
            var prickCtrl = pickCounter + 1;
            var count = 1;
            while (prickCtrl > totalTeams) {
                prickCtrl = prickCtrl - totalTeams;
                count++;
            }
            //console.log("(count % 2): " + (count % 2));
            if ((count % 2) == 0) {
                //walk DOWN the draft order
                prickCtrl = (totalTeams - prickCtrl) + 1;
                var currentPickTeamID = draftOrderIDs[prickCtrl - 1];
                var currentPickTeamName = draftOrderNames[prickCtrl - 1];
                $("#draftFantasyTeamLogo").attr("src", "/Images/TeamLogos/" + draftOrderTeamLogos[prickCtrl - 1]);
            }
            else {
                //walk UP the draft order
                var currentPickTeamID = draftOrderIDs[prickCtrl - 1];
                var currentPickTeamName = draftOrderNames[prickCtrl - 1];
                $("#draftFantasyTeamLogo").attr("src", "/Images/TeamLogos/" + draftOrderTeamLogos[prickCtrl - 1]);
            }
            currentDraftOrderInt = prickCtrl - 1;
            firstLogin = false;
            console.log("-----First Time Loggin In![OUT]-----");
        }

        //Reset draft order table
        for (var i = 1; i <= totalTeams + 1; i++) {
            $("#trDraftOrderNum" + i).removeClass("OnTheClock");
        }
        currentTeamPickingName = draftOrderNames[currentDraftOrderInt];
        $("#draftFantasyTeamLogo").attr("src", "/Images/TeamLogos/" + draftOrderTeamLogos[currentDraftOrderInt]);
        $("#TeamDraftingMeow").text(currentTeamPickingName);
        $("#trDraftOrderNum" + (currentDraftOrderInt + 1)).addClass("OnTheClock");
        //console.log("currentTeamPickingName: " + currentTeamPickingName);

        //Allow user who's pick it is to click the draft button
        currentTeamPickingID = draftOrderIDs[currentDraftOrderInt];
        //console.log("currentTeamPickingID: " + currentTeamPickingID);
        //console.log("$(#userTeamID).val(): " + $("#userTeamID").val());
        if ($("#userTeamID").val() == currentTeamPickingID) {
            $(".disableMe").prop("disabled", false);
        } else {
            $(".disableMe").prop("disabled", true);
        }

        //start the draft clock
        StartPickClock();
    }
}

function StartPickClock() {
    console.log("------StartPickClock-------");
    var counter = 0;
    z = setInterval(function () {
        //console.log("StartPickClock iteration#" + counter);
        // Get todays date and time
        nowPick = new Date();

        //Find distance between pick time and now
        //console.log("nowPick.Date(): " + nowPick);
        //console.log("pickTime.Date(): " + pickTime);
        var distance = pickTime - nowPick;
        //console.log("distance: " + distance);

        //testing
        //console.log("nowPick.getTime(): " + nowPick.getTime());
        //console.log("pickTime.getTime(): " + pickTime.getTime());
        //distance = pickTime.getTime() - nowPick.getTime();
        //console.log("distance: " + distance);

        // Time calculations for days, hours, minutes and seconds
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Display the result in the element with id="demo"
        document.getElementById("pickTimer").innerHTML = minutes + "m " + seconds + "s ";

        // If the count down is finished, write some text
        counter++;
        if (distance <= 0) {
            console.log("-----StartPickClock Timer Finished-----")
            var index = 0;
            if (pickCounter < 0) {
                index = 0;
            } else {
                index = pickCounter + 1;
            }
            currentDraftOrderInt = parseInt(draftOrderGeneric[index]);

            //Add skater picked to team
            //console.log("Index: " + index);
            if(index > 0){
                var rowNum = $("#skaterID").val();
                //console.log("AddSkaterToFantasyTeam - skaterID: " + rowNum);
                $("#row" + rowNum).addClass("hidden");
                AddSkaterToFantasyTeam();
            } else {
                //console.log("SetNextRankedSkater - skaterID: " + rowNum);
                SetNextRankedSkater();
            }

            //Update current pick team id
            //console.log("currentDraftOrderInt: " + currentDraftOrderInt);
            if (currentDraftOrderInt > -1) {
                UpdateCurrentTeamDrafting(draftOrderIDs[currentDraftOrderInt]);
                //console.log("draftOrderIDs[currentDraftOrderInt]: " + draftOrderIDs[currentDraftOrderInt]);
                $("#teamID").val(draftOrderIDs[currentDraftOrderInt]);
            }

            clearInterval(z);
            KickOffPickClock();
        }
    }, 1000);
}

//---------------------------------
//Add minutes to given date
function addMinutes(date, minutes) {
    return new Date(date.getTime() + (minutes * 60000));
}

//Add seconds to given date
function addSeconds(date, seconds) {
    return new Date(date.getTime() + (seconds * 1000));
}

//Function called when "Draft Me" button is clicked for the skater
function DraftMe(skaterID) {
    $("#rowNum").val(skaterID);
    $("#skaterID").val(skaterID);
    $("#triggerSignalR").click();
}

//Add skater to fantasy team's roster
function AddSkaterToFantasyTeam() {
    console.log("AddSkaterToFantasyTeam [POST]");
    //$("#scrollToTableHeader").val("false");
    var teamID = $("#teamID").val();
    var seasonID = $("#seasonID").val();
    var skaterID = $("#skaterID").val();
    var userID = $("#userID").val();
    var userTeamID = $("#userTeamID").val();
    var random = $("#Random").val();
    console.log("AddSkaterToFantasyTeam pickCounter: " + pickCounter);
    console.log("Random: " + $("#Random").val());
    console.log("userTeamID: " + $("#userTeamID").val());
    console.log("userID: " + $("#userID").val());
    $.ajax({
        type: "POST",
        url: "/Draft/AddSkaterToFantasyTeam",
        data: {
            teamID: teamID,
            seasonID: seasonID,
            skaterID: skaterID,
            draftPickNumber: pickCounter + 1,
            userID: userID,
            userTeamID: userTeamID,
            random: random
        },
        success: function (response) {
            console.log(response);
            //Set next skater ranked as the skater to be chosen next
            SetNextRankedSkater();
        }
    })
}

//Add skater to fantasy team's roster
function UpdateCurrentTeamDrafting(teamID) {
    var seasonID = $("#seasonID").val();
    $.ajax({
        type: "POST",
        url: "/Draft/UpdateCurrentTeamDrafting",
        data: { teamID: teamID, seasonID: seasonID },
        success: function (response) {
            console.log(response);
        }
    })
}

function SetNextRankedSkater() {
    //console.log("SetNextRankedSkater [GET]");
    var seasonID = $("#seasonID").val();
    $.ajax({
        type: "GET",
        url: "/Draft/GetNextRankedSkaterID",
        data: { seasonID: seasonID },
        success: function (response) {
            //console.log("/Draft/GetNextRankedSkaterID SUCCESS!: " + response);
            var rowName = $("[name='tr" + response + "'").attr("id");
            //console.log("rowName: " + rowName);
            var rowNum = rowName.substring(3);
            //console.log("rowNum: " + rowNum);
            DraftMe(rowNum, response);
        }
    })
}