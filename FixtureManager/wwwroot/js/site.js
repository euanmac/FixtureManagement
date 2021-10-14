// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function nextWeekdayDate(date, day_in_week) {
    var ret = new Date(date || new Date());
    //ret.setDate(ret.getDate() + (day_in_week - 1 - ret.getDay() + 7) % 7 + 1);

    var diff = (day_in_week - ret.getDay());
    ret.setDate(ret.getDate() + (diff <= 0 ? diff + 7 : diff));

    return ret;
}

//function prevWeekdayDate(date, day_in_week) {
//    var ret = new Date(date || new Date());
//    //ret.setDate(ret.getDate() + (day_in_week - 1 - ret.getDay() + 7) % 7 + 1) - 7;

//    var diff = (day_in_week - ret.getDay());
//    ret.setDate(ret.getDate() + (diff <= 0 ? diff + 7 : diff));

//    return ret;
//}

function prevWeekdayDate(date, day_in_week) {
    var ret = new Date(date || new Date());
    //ret.setDate(ret.getDate() + (day_in_week - 1 - ret.getDay() + 7) % 7 + 1) - 7;

    var diff = (ret.getDay() - day_in_week);
    ret.setDate(ret.getDate() + -1 * (diff <= 0 ? diff + 7 : diff));

    return ret;
}

/*Fri = 5
 *
 * day in week = 6 (Sat)
 * 6 - 1 - 5 (Fri) - 7
 
 *
 *
 *
 *Mon (1)
 *
 * Day in week = 6 (Sat)
 * -1 = 5
 * -1 (Mon) = 4
 *- 7 = -3
 * -2
 *
 *
 * Sat = 6
 * -1 = 5
 *5 -6 = -1
 * -1 - 7 = -8
 * 
 *
 *
 *
 * current day = 4 (Thur)
 * next day 6 (Sat)
 *
 * 6 - 4 = 2 - add 2
 *
 */