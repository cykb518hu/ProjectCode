jQuery(document).ready(function ($) {

    $("#btn-add-meetingNote").click(function () {

        var str = "";
        str = '<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" required="required"></textarea></td><td>' + new Date().Format("yyyy-MM-dd") + '</td><td>' + new Date().Format("yyyy-MM-dd") + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status">Added</span><span style="display:none" class="note-guid">' + guid() + '</span><span style="display:none" class="note-old-value"></span></td></tr>';
        $("#meetingNote-table >tbody").append(str);
    });
    $("#btn_Save_MeetingNote").click(function () {
        var noteArr = new Array();
        var docGuid = $("#hid_noteDocId").val();
        $("#meetingNote-table > tbody  > tr").each(function () {
            var status = $(this).find(".note-status").html();
            var noteStr = $(this).find(".note-text").val();
            if (noteStr.length == 0) {
                alert("note can't be empty");
                return;
            }
            if (status.length > 0)
            {
                var note = {
                    Note: $(this).find(".note-text").val(),
                    Guid: $(this).find(".note-guid").html(),
                    Status: $(this).find(".note-status").html(),
                    DocGuid:docGuid
                };
                noteArr.push(note);
            }
        })
        updateMeetingNoteToDb(noteArr);
    });
 
});

function updateMeetingNoteToDb(noteArr)
{
    $.ajax({
        url: '/MeetingNote/SaveMeetingNotes',
        data: { notes: JSON.stringify(noteArr) },
        dataType: 'json',
        type: "POST",
        success: function (result) {
        },
        complete: function (XMLHttpRequest, textStatus) {
            $("#myNotesModal").modal('hide');
        },
        error: function (xhr, textStatus, errorThrown) {

        }
    });
}
function loadNoteData(docId)
{
    $("#meetingNote-table >tbody").html("");
    $.ajax({
        url: '/MeetingNote/GetMeetingNotes',
        data: { docGuid: docId },
        dataType: 'json',
        type: "GET",
        success: function (result) {
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    var data = result[i];
                    var str = "";
                    str = '<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" >' + data.Note + '</textarea></td><td>' + data.CreateDate + '</td><td>' + data.ModifyDate + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status"></span><span style="display:none" class="note-guid">' + data.Guid + '</span><span style="display:none" class="note-old-value">' + data.Note + '</span></td></tr>';
                    $("#meetingNote-table >tbody").append(str);
                }
            }
            $("#myNotesModal").modal('show');

        },
        complete: function (XMLHttpRequest, textStatus) {
        },
        error: function (xhr, textStatus, errorThrown) {

        }
    });
}
function deleteMeetingNote(obj) {
    $(obj).parent().parent().hide();
    var status = $(obj).parent().parent().find(".note-status").html();
    if (status == "Added") {
        $(obj).parent().parent().find(".note-status").html("");
    }
    else {
        $(obj).parent().parent().find(".note-status").html("Deleted");
    }
}
function modifyMeetingNote(obj) {
    var note = $(obj).val();
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

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}