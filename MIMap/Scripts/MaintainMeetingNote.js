jQuery(document).ready(function ($) {

    $("#btn-add-meetingNote").click(function () {
        var str = "";
        str = '<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" ></textarea></td><td>' + new Date().Format("yyyy-MM-dd") + '</td><td>' + new Date().Format("yyyy-MM-dd") + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status">Added</span><span style="display:none" class="note-guid"></span><span style="display:none" class="note-old-value"></span></td></tr>';
    });

    function deleteMeetingNote(obj) {
        $(obj).parent().parent().hide();
        var status = $(obj).parent().parent().find(".note-status").html();
        if (status == "Added") {
            $(obj).parent().parent().find(".note-status").html("none");
        }
        else {
            $(obj).parent().parent().find(".note-status").html("Deleted");
        }
    }
    function modifyMeetingNote(obj) {
        var note = $(this).val();
        var status = $(obj).parent().parent().find(".note-status").html();
        if (status == "Added") {
            return;
        }
        var oldNote = $(obj).parent().parent().find(".note-old-value").html();

        if (note != oldNote) {
            $(obj).parent().parent().find(".note-status").html("Modified");
        }
        if (note == oldNote) {
            $(obj).parent().parent().find(".note-status").html("");
        }
    }
});

Date.prototype.Format = function (fmt) { //author: meizz
    var o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "h+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}