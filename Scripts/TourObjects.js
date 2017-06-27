
function startTutorial(tour) {
    tour.init();
    tour.start();
    tour.restart();
}

function defaultPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: ".main",
            title: "Goal Group",
            placement: "bottom",
            content: "This is a Goal Group. Each box represents a goal. It shows the Title, last activity completed date and number " +
                     "of activities in the goal."
        }]
    });
    return tour;
}

function listActivityPageTour() {
    var tour = new Tour({
        backdrop: true,
        onEnd: function (tour) {
            $("#goalItems").toggle();
            $("#goalItems").removeAttr('style');
            $("#actItems").toggle();
            $("#actItems").removeAttr('style');
            $("#supItems").toggle();
            $("#supItems").removeAttr('style');
            $("#accountItems").toggle();
            $("#accountItems").removeAttr('style');
        },
        steps: [{
            element: "#menubar",
            title: "Menu Bar",
            content: "The Menu Bar holds all the functions that you can do with Sweet Goals. Bolded items are active."
        },
        {
            element: "#goalSection",
            title: "Goal List",
            content: "Lists the type of goals your viewing. Working, Finished, Suggested"

        },
        {
            element: "#activitySection",
            title: "Activity Section",
            content: "The Activity Section holds all your activities for a given goal. Activities are the actions you " +
                     "take to complete the goal. The activities are listed in the order in which they were completed " +
                     "with the most recent one at the top. This gives a step-by-step process of how the goal was " +
                     "achieved. The activities also make Sweet Goals a great place to create lists where you want to " +
                     "remember the order of things (example book, or movie list). When you are viewing the 'Created' " +
                     "Activities you can reorder the activities by clicking on them and moving them up and down. " +
                     "Click the 'Save Order' button to save the order of the 'Created' Activities."
        },
        {
            element: "#FeaturedContent_goalDetailTable",
            title: "Goal Summary",
            content: "Gives basic information about the goal you are viewing.",
            placement: "bottom"
        },
        {
            element: "#FeaturedContent_supportTable",
            title: "Support Table",
            content: "Lists the email address of the people who are supporting the goal. These people are here to " +
                     "give postive reinforcement and provide a person to talk about the goal with. Every time you " +
                     "complete an activity they recieve an email about the activity and are given a chance to " +
                     "provide feedback on that activity. ",
            placement: "bottom"
        },
    //************* Goal Menu
        {
            element: "#goalMenu",
            title: "Goal Menu",
            content: "Controls for goals.",
            onShow: function (tour) {
                $("#goalItems").css('visibility', 'visible');
                $("#goalItems").css('background-color', 'royalblue');
            },
            onPrev: function (tour) {
                $("#goalItems").toggle();
                $("#goalItems").removeAttr('style');
            }
        },
        {
            element: "#createGoal",
            title: "Create Goal",
            content: "Goes to the Create Goal Page so you can create a goal!"
        },
        {
            element: "#goalComplete",
            title: "Complete Goal",
            content: "When you are done working on a goal. Click the 'Complete Goal' to mark the Goal complete. " +
                     "Will move the goal from the 'Working List' to the 'Finished List'"
        },
        {
            element: "#goalRestart",
            title: "Restart Goal",
            content: "If you decide to ever pick up a goal again that you have finished then click the 'Restart Goal' " +
                     "item. It will move the goal from the 'Finished List' to the 'Working List' and you will be able " +
                     "to continue working on it. "
        },
        {
            element: "#goalReasons",
            title: "Goal Reasons",
            content: "A place you can write down all the reasons your doing this goal."
        },
        {
            element: "#goalSummary",
            title: "Summary",
            content: "Gives Statistics about the goal."
        },
        {
            element: "#goalPublic",
            title: "Publish/Private Switch",
            content: "This is a Publish/Private Switch. When the goal is in a 'Private' state a 'Publish' item will " +
                     "be visable. Clicking on 'Publish' will put the goal on the Public List and let all users on " +
                     "Sweet Goals view it and comment on activities. When the goal is on the Public List a " +
                     "'Private' item will be visiable. Clicking on 'Private' will make the goal only viewable to you " +
                     "and the people who are supporting the goal."
        },
        {
            element: "#workList",
            title: "Working Goals",
            content: "Working Goals will list the goals that are currently being worked on."
        },
        {
            element: "#completeList",
            title: "Finished Goals",
            content: "Finished Goals will list the goals that have been completed."
        },
        {
            element: "#supportList",
            title: "Supporting Goals",
            content: "Supporting Goals will list the goals that your currently supporting. It will also give the " +
                     "user name of the person that the goal belongs to",
        },
        {
            element: "#publicList",
            title: "Public Goals",
            content: "These are goals that users have listed as public. You can view everything about them and " +
                     "comment on them but you can't change them. When a goal becomes private it is removed from " +
                     "this list. Viewing these goals is great way to understand what Sweet Goals is about and how " +
                     "it works. Plus, you get to see what other people are working on and how they are doing it.",
            onHide: function (tour) {
                $("#goalItems").toggle();
                $("#goalItems").removeAttr('style');
            },
            onPrev: function (tour) {
                $("#goalItems").css('visibility', 'visible');
                $("#goalItems").css('background-color', 'royalblue');
            }
        },

    //************** Activity Menu
        {
            element: "#actMenu",
            title: "Activity Menu",
            content: "Controls the activities that are on the goal.",
            onShow: function (tour) {
                $("#actItems").css('visibility', 'visible');
                $("#actItems").css('background-color', 'royalblue');
            },
            onPrev: function (tour) {
                $("#actItems").toggle();
                $("#actItems").removeAttr('style');
                $("#goalItems").css('visibility', 'visible');
                $("#goalItems").css('background-color', 'royalblue');
            },

        },
        {
            element: "#createActivity",
            title: "Create Activity",
            content: "Goes to the create activity page"
        },
        {
            element: "#createActList",
            title: "Created Activity List",
            content: "Switches the Activity Section to list the created activities for the goal. You can reorganize " +
                     "the created activities by clicking on the activity and moving it up and down. To save the " +
                     "order click the 'Save Order' button."
        },
        {
            element: "#completeActList",
            title: "Completed Activity List",
            content: "Switches the Activity Section to list the completed activities for the goal. They are displayed " +
                     "in the order in which they were completed with the most recent completed activity at the top."
        },
        {
            element: "#suggestActList",
            title: "Suggested Activity",
            content: "Switches the Activity Section to list the suggested activities for the goal. These are " +
                     "activities that the supporters for the goal have suggested that can be done for the goal.",
            onHide: function (tour) {
                $("#actItems").toggle();
                $("#actItems").removeAttr('style');
            },
            onPrev: function (tour) {
                $("#actItems").css('visibility', 'visible');
                $("#actItems").css('background-color', 'royalblue');
            },
        },
    //************* Support Menu
        {
            element: "#supportMenuItem",
            title: "Support Menu Item",
            content: "Controls for the supporters. Click on 'Supporters' will go to the Support Page",
            onShow: function (tour) {
                $("#supItems").css('visibility', 'visible');
                $("#supItems").css('background-color', 'royalblue');
            },
            onPrev: function (tour) {
                $("#supItems").toggle();
                $("#supItems").removeAttr('style');
                $("#actItems").css('visibility', 'visible');
                $("#actItems").css('background-color', 'royalblue');
            },
        },
        {
            element: "#supAddAccount",
            title: "Add Support to Account",
            content: "Brings up a window so that a supporter can be attached to your account. Enter their email " +
                     "address to send out an support request."
        },
        {
            element: "#supAddGoal",
            title: "Add Support to Goal",
            content: "Brings up a window so that you can add a supporter to the goal and account. Enter their email " +
                     "address to send them a request to support the goal",
            onHide: function (tour) {
                $("#supItems").toggle();
                $("#supItems").removeAttr('style');
            },
            onPrev: function (tour) {
                $("#supItems").css('visibility', 'visible');
                $("#supItems").css('background-color', 'royalblue');
            },

        },
    //************** Settings
        {
            element: "#accountMenuItem",
            title: "Account Settings",
            content: "Controls for your account. Clicking on Account will bring up the Account Settings Page.",
            onShow: function (tour) {
                $("#accountItems").css('visibility', 'visible');
                $("#accountItems").css('background-color', 'royalblue');
            },
            onPrev: function (tour) {
                $("#accountItems").toggle();
                $("#accountItems").removeAttr('style');
                $("#supItems").css('visibility', 'visible');
                $("#supItems").css('background-color', 'royalblue');
            },
        },
        {
            element: "#setItem",
            title: "Account Settings",
            content: "Controls your account. Clicking on Settings will bring up the Account Settings Page.",
            onHide: function (tour) {
                $("#accountItems").toggle();
                $("#accountItems").removeAttr('style');
            },
        },
    //**************** 
        {
            element: "#tutorial",
            title: "Tutorial",
            content: "Each page has a Tutorial and it explains the functions on that page.",
            onShow: function (tour) {
                $("#aboutItems").css('visibility', 'visible');
                $("#aboutItems").css('background-color', 'royalblue');
            },
            onHide: function (tour) {
                $("#aboutItems").toggle();
                $("#aboutItems").removeAttr('style');
            },
            onPrev: function (tour) {
                $("#accountItems").css('visibility', 'visible');
                $("#accountItems").css('background-color', 'royalblue');
            },
        },
        {
            element: "#donateImg",
            title: "Donate Button",
            content: "This is my weekend warrior project. If this helps you and you have the means and desire to " +
                     "donate. I would really like that!",
            placement: "bottom"
        }
        ]
    });
    return tour;
};

function createGoalPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#createGoalTable",
            title: "Goal Table",
            content: "Writing down your goal is the most important thing you can do. You need to have a plan and it " +
                     "needs to be good. When filling this section out try and develop a good idea what you're doing " +
                     "and how long this should take you to do. The times will never be exact but have a good idea of " +
                     "how long you expect to work on a general activity for this goal."
        },
        {
            element: "#FeaturedContent_goalTitleText",
            title: "Goal Title",
            content: "Enter the title of your goal."
        },
        {
            element: "#FeaturedContent_goalDescText",
            title: "Goal Description",
            content: "Enter a description for your goal. You should put some thought into why your doing this and " +
                     "what exactly are you trying to achieve."
        },
        {
            element: "#FeaturedContent_calenderBox",
            title: "Completion Date",
            content: "Enter the date which you plan to complete the goal. "
        },
        {
            element: "#actSetup",
            title: "Activity Setup",
            content: "This section is used to setup the plan for the activities. How often do you plan to work on " +
                     "this goal and how long should each activity take? This section should read like the " +
                     "following examples. Example 1 : 1 time per week for 2 hours. Example 2: 3 times per month for " +
                     "30 minutes. Example 3: 1 time per day for 45 minutes"
        },
        {
            element: "#FeaturedContent_freqBox",
            title: "Frequency",
            content: "How many times do you plan to do an activity associated with this goal within a day/week/month " +
                     "period"
        },
        {
            element: "#FeaturedContent_freqDropDown",
            title: "Frequency Period",
            content: "How long is the period. day/week/month. "
        },
        {
            element: "#FeaturedContent_timeText",
            title: "Activity Length",
            content: "How long will each activity be. "
        },
        {
            element: "#tutorial",
            title: "Tutorial",
            content: "Activates the Tutorial",
            placement: "right",
            onShow: function (tour) {
                $("#aboutItems").css('visibility', 'visible');
                $("#aboutItems").css('background-color', 'blue');
            },
            onHide: function (tour) {
                $("#aboutItems").toggle();
                $("#aboutItems").removeAttr('style');
            },
        },
        ]
    });
    return tour;
}

function goalReasonsPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#FeaturedContent_goalReasonLabel",
            title: "Goal Reasons",
            placement: "bottom",
            content: "Goal Reasons gives you a place to write down all the reasons that you're doing a goal. After " +
                     "all what's the point of spending hours doing something if there isn't a clear defined reason " +
                     "for it? It's important to stay focused on the benefits of your goals. "
        },
        {
            element: "#reasonList",
            title: "Reason List",
            content: "All the reasons will be listed in the Reason List. List as many as you want. To reorder them " +
                     "just click on the reason and drag up and down. Click on the 'x' icon to remove the reason",
            placement: "bottom"
        },
        {
            element: "#addButton",
            title: "Add Reason Button",
            content: "Adding a reason is simple. Just type the reason in the textbox above and then click the 'Add " +
                     "Reason' button and the reason will be added to the Reason List",
            placement: "bottom"
        },
        {
            element: "#FeaturedContent_saveReasons",
            title: "Save Reason Button",
            content: "Click the 'Save Reason' button to save the Reason List.",
            placement: "bottom"
        },

        ]
    });
    return tour;
};

function createActivityPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#activityTable",
            title: "Create Activity Table",
            content: "This is the place to create activities for goals. Activities are the means of how to complete " +
                     "goals. They give you a place to write down exactly what you did and how you did it and your " +
                     "thoughts about the activity. As you add more and more activities to your goal it also provides " +
                     "a history of how you completed the goal. This is great because if you ever want to revisit the " +
                     "goal you now have the history and can see exactly how you did it. It's great place to start " +
                     "research if you try and complete another goal that is similar. "
        },
        {
            element: "#FeaturedContent_actTitleText",
            title: "Activity Title",
            content: "Give the activity a title. This should be a general theme of what you plan to do during your " +
                     "activity."

        },
        {
            element: "#FeaturedContent_actDescText",
            title: "Activity Description",
            content: "The Activity Description gives you a place to describe what you are doing during this activity. " +
                     "This is also a good place to define how this will help you complete your overall goal. "
        },
        {
            element: "#FeaturedContent_startCheckBox",
            title: "Start Box",
            content: "Check to start the goal when you click the 'Create' Button. If this is left unchecked then it " +
                     "will just create the activity and put it on the Activity Create List."
        },
         {
             element: "#tutorial",
             title: "Tutorial",
             content: "Activates the Tutorial",
             placement: "right",
             onShow: function (tour) {
                 $("#aboutItems").css('visibility', 'visible');
                 $("#aboutItems").css('background-color', 'blue');
             },
             onHide: function (tour) {
                 $("#aboutItems").toggle();
                 $("#aboutItems").removeAttr('style');
             },
         },
        ]
    });
    return tour;
};

function detailActivityPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#detailGroup",
            title: "Acitivity Detail Table",
            content: "This is the detail of your activity. The Start and Delete activity buttons will appear in the " +
                     "left hand corner when the activity is part of the Created Activity List. Once an activity has " +
                     "been started. It gets transfered to the Working Activity List and can't be deleted. The stop " +
                     "activity button will appear at the bottom when the activity has been started. All input boxes" +
                     "on this page are optional.",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_goalTitle",
            title: "Goal Title",
            content: "Title of the goal that the activity belongs to.",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_actTitle",
            title: "Activity Title",
            content: "Title of the activity",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_Start",
            title: "Start Time",
            content: "Server time that you started the activity.",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_End",
            title: "End Time",
            content: "Serve time you stopped working on the activity",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_Difference",
            title: "Time Difference",
            content: "The time difference between the Start and End time.",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_Date_Completed",
            title: "Complete Date",
            content: "Date the activity was completed on",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_Date_Created",
            title: "Created Date",
            content: "Date the activity was created.",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_addPicIcon",
            title: "Add Pictures Icon",
            content: "Click on the 'Add Pictures Icon' to add pictures to your activity. This helps to provide proof " +
                     "that you actually did do the activity. It will also give you a scrap book of what you've done " +
                     "with your goal",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_actDesc",
            title: "Activity Description",
            content: "Description of the activity was completed on",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_actGood",
            title: "Good Input",
            content: "Provides a place to put down your thoughts on how the activity went. You can put whatever you " +
                     "want, but what was good?",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_actBad",
            title: "Bad Input",
            content: "Provides a place to put down your thoughts on how the activity went. You can put whatever you " +
                     "want, but what could you improve?",
            placement: "bottom",
        },
        {
            element: "#FeaturedContent_manualBox",
            title: "Manual Time",
            content: "This allows you to input a time that is different than the calculated time difference by the " +
                     "server. This is mainly used when you do an activity, but you aren't close to your computer " +
                     "when you start and/or end it. Allows you to enter an activity that maybe you forgot about " +
                     "or you couldn't get to your computer",
            placement: "bottom",
        },
        ]
    });
    return tour;
}

function supportPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#FeaturedContent_supportTable",
            title: "Support Table",
            content: "It gives an easy place to store a pool of people that might be willing to support your goal."
        },
        {
            element: "#FeaturedContent_goalSupportTable",
            title: "Support Table",
            content: "These are the goal supporters. They are following your goal and they can see everything that " +
                     "you do with your goal. They are to provide you with the motovation and the confirmation that " +
                     "you are actually completing the activities and completing the goal that you set out to do. They " +
                     "don't do the work, they just make sure your doing the work and give you someone to talk to " +
                     "about the goal. They can also give you suggested activities which you can choose to do or not. " +
                     "They can also comment on your activities. "
        },
        ]
    });
    return tour;
};

function settingsPageTour() {
    var tour = new Tour({
        backdrop: true,
        steps: [{
            element: "#FeaturedContent_settingsTable",
            title: "Account Settings",
            content: "This page deals with all the settings that deal with your account",
        },
        {
            element: "#FeaturedContent_unsubscribeBox",
            title: "Unsubscribe Check Box",
            content: "This will turn off all emails from Sweet Goals to the accounts email address. This box is also " +
                     "on the supporter response page. If the user checks it there then they will have to come into " +
                     "their settings to change it.",
        },
        {
            element: "#FeaturedContent_emailBox",
            title: "Account Email",
            content: "Email address associated with the account.",
        },
        {
            element: "#FeaturedContent_updateButton",
            title: "Update Button",
            content: "Saves the settings to the account.",
        },

        ]
    });
    return tour;
};