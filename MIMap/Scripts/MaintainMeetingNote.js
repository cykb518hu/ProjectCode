jQuery(document).ready(function ($) {

    $("#myNotesModal").on("click", ".tagDisplay", function () {

        if ($(this).hasClass("noteSelected")) {
            $(this).removeClass("noteSelected");
        }
        else {
            $(this).addClass("noteSelected");
        }
        var allDisplayTags = "";
        $(".tagDisplay").each(function () {
            if ($(this).hasClass("noteSelected")) {
                var str = $(this).attr("data-tag");
                if (str.length > 0) {
                    allDisplayTags += str;
                    allDisplayTags += ","
                }
            }
        });
        if (allDisplayTags.length > 0) {
            allDisplayTags = allDisplayTags.substring(0, allDisplayTags.length - 1);
        }
        var tagArr = allDisplayTags.split(",");
        tagArr = $.unique(tagArr);
        $("#meetingNote-table > tbody  > tr").each(function () {
            var matched = false;
            var currentTags = $(this).find(".input-tags").val();
            if (currentTags.length > 0) {
                if (allDisplayTags.length > 0) {
                    for (var i = 0; i < tagArr.length; i++) {
                        if (currentTags.indexOf(tagArr[i]) >= 0) {
                            matched = true;
                        }
                    }
                }
                else {
                    matched = false;
                }
                if (matched) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            }
           
        });


    });

    $("#btn-add-meetingNote").click(function () {


        $tr = $('<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" required="required"></textarea></td><td class="td-tag"></td><td>' + new Date().Format("yyyy-MM-dd") + '</td><td>' + currentUser + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status">Added</span><span style="display:none" class="note-guid">' + guid() + '</span><span style="display:none" class="note-old-value"></span></td></tr>');
        $input = $('<input type="text" class="input-tags" value="" >');
        $tr.find("td.td-tag").append($input);
        $("#meetingNote-table >tbody").append($tr);
        $input.selectize({
            plugins: ['remove_button'],
            persist: false,
            valueField: 'tag',
            labelField: 'tag',
            searchField: 'tag',
            options: defaultTags,
            create: true
        });
    });
    $("#btn_Save_MeetingNote").click(function (e) {
        e.preventDefault();
        var noteArr = new Array();
        var docGuid = $("#hid_noteDocId").val();
        $("#meetingNote-table > tbody  > tr").each(function () {
            var status = $(this).find(".note-status").html();
            var noteStr = $(this).find(".note-text").val();
            if (noteStr.length == 0) {
                if (status == "Added") {
                    errorTips("note can't be empty");
                    return;
                }
            }
            if (status.length > 0)
            {
               // var oldNote = $(this).find(".note-old-value").html();
               // if (noteStr != oldNote || status == "Deleted") {
                    var note = {
                        Note: $(this).find(".note-text").val(),
                        Guid: $(this).find(".note-guid").html(),
                        Tags: $(this).find(".input-tags").val(),
                        Status: $(this).find(".note-status").html(),
                        DocGuid: docGuid
                    };
                    noteArr.push(note);
              //  }
              
            }
        })
        updateMeetingNoteToDb(noteArr);
    });
 
});

function updateMeetingNoteToDb(noteArr)
{
    if(noteArr.length>0)
    {
        $.ajax({
            url: '/MeetingNote/SaveMeetingNotes',
            data: { notes: JSON.stringify(noteArr) },
            dataType: 'json',
            type: "POST",
            success: function (result) {
                updateDocLevelNoteButton(result, noteArr[0].DocGuid);
                successTips();
            },
            complete: function (XMLHttpRequest, textStatus) {
                $("#myNotesModal").modal('hide');
            },
            error: function (xhr, textStatus, errorThrown) {

            }
        });
    }
    else {
        $("#myNotesModal").modal('hide');
    }
}
function updateDocLevelNoteButton(amount, docGuid) {
    $("button[data-docid='" + docGuid + "']").eq(0).removeClass("btn-default").removeClass("btn-success");
    if (amount > 0) {
        $("button[data-docid='" + docGuid + "']").eq(0).addClass("btn-success");
    }
    else {
        $("button[data-docid='" + docGuid + "']").eq(0).addClass("btn-default");
    }
}
function loadNoteData(docId)
{
    $("#myNotesModal").find(".options").html("");
    var allDisplayTags = "";
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
                    $tr = $('<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" >' + data.Note + '</textarea></td><td class="td-tag"></td><td>' + data.CreateDate + '</td><td>' + data.ModifyUser + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status">Modified</span><span style="display:none" class="note-guid">' + data.Guid + '</span><span style="display:none" class="note-old-value">' + data.Note + '</span></td></tr>');
                    $input = $('<input type="text" class="input-tags" value="' + data.Tags + '" >');
                    $tr.find("td.td-tag").append($input);
                   // str = '<tr><td><textarea class="form-control note-text" rows="2" onchange="modifyMeetingNote(this); return false" >' + data.Note + '</textarea></td><td><input type="text" class="input-tags demo-default" value="" ></td><td>' + data.CreateDate + '</td><td>' + data.ModifyUser + '</td><td><button type="button" class="btn btn-default glyphicon glyphicon-remove" onclick="deleteMeetingNote(this); return false"></button></td><td><span style="display:none" class="note-status"></span><span style="display:none" class="note-guid">' + data.Guid + '</span><span style="display:none" class="note-old-value">' + data.Note + '</span></td></tr>';
                    $("#meetingNote-table >tbody").append($tr);
                    $input.selectize({
                        plugins: ['remove_button'],
                        persist: false,
                        valueField: 'tag',
                        labelField: 'tag',
                        searchField: 'tag',
                        options: data.AllTags,
                        create: true
                    });
                    if (data.Tags.length > 0) {
                        allDisplayTags += data.Tags;
                        allDisplayTags += ","
                    }
                }
            }
            if (allDisplayTags.length > 0) {
                allDisplayTags = allDisplayTags.substring(0, allDisplayTags.length - 1);
                var tagArr = allDisplayTags.split(",");
                tagArr = $.unique(tagArr);
                for (var i = 0; i < tagArr.length; i++) {
                    var tagStr = '<div class="noteSelected tagDisplay" data-tag="' + tagArr[i] + '">' + tagArr[i] + '</div>';
                    $("#myNotesModal").find(".options").append(tagStr);
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
        $(obj).parent().parent().remove();
    }
    else {
        $(obj).parent().parent().find(".note-status").html("Deleted");
    }
}
function modifyMeetingNote(obj) {
    //var note = $(obj).val();
    //var status = $(obj).parent().parent().find(".note-status").html();
    //if (status == "Added") {
    //    return;
    //}
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