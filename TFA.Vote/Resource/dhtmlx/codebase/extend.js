dhtmlXCalendarObject.prototype.langData["cn"] = {
    dateformat: '%Y-%m-%d',
    monthesFNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
    monthesSNames: ["一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二"],
    daysSNames: ["日", "一", "二", "三", "四", "五", "六"],
    weekstart: 1,
    weekname: "W",
    today: "今天",
    clear: "清空"
};
dhtmlXCalendarObject.prototype.lang = "cn";

function getQueryStringByName(name) {

    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));

    if (result == null || result.length < 1) {

        return "";

    }

    return result[1];

}