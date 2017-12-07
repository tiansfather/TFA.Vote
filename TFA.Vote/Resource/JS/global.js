var TF = {
    Code: { "Error": "-1", "OK": "0" },
    ajaxError: function (xhr, ajaxOptions, thrownError) {
        try {
            var json = $.parseJSON(xhr.responseText);
            TF.alert(json.errMsg);
            return;
        } catch(e){
        
        }
        var tip = "<div>Http status: " + xhr.status + " " + xhr.statusText + "</div>"
				+ "<div>ajaxOptions: " + ajaxOptions + "</div>"
				+ "<div>thrownError: " + thrownError + "</div>"
				+ "<div>" + xhr.responseText + "</div>";
        TF.alert(tip);
    },
    ajaxDone: function (json) {
        layer.close(window.loadingLayerIndex);
        var title = "";        
        if (json.forwardUrl && !json.errMsg) {
            location.href = json.forwardUrl;
            return;
        }
        TF.alert(json.errMsg, function () {
            layer.close(window.alertLayerIndex);
            if (json.forwardUrl) {
                location.href = json.forwardUrl;
            }
            if (json.errCode == TF.Code.OK) {

            } else {
            }
            if (json.callback) {
                var cb = new Function(json.callback);
                console.log(cb);
                cb();
            }
        });

    },
    loading: function (msg) {
        window.loadingLayerIndex=layer.msg(msg||'Loading', { icon: 16, time: 0, shade: [0.4, '#393D49'] });
    },
    alert: function (msg, fn) {
        window.alertLayerIndex=layer.alert(msg, {closeBtn:0}, fn);
    },
    ajaxDo: function (url, callback, options) {
        var opt = {
            type: 'post',
            url: url,
            async: true,
            dataType: "json",
            cache: false,
            success: callback || TF.reload,
            error: TF.ajaxError
        };
        opt = $.extend(opt, options);
        $.ajax(opt);
    },
    reload: function () {
        location.reload();
    }
}
/**
* 带文件上传的ajax表单提交
* @param {Object} form
* @param {Object} callback
*/
function iframeCallback(form) {
    var $form = $(form), $iframe = $("#callbackframe");
    //表单提交前的自定义函数
    var beforeFn = $(form).attr("before");
    if (beforeFn) {
        beforeFn = eval(beforeFn);
        if (!beforeFn($(form))) {
            return false;
        }
    }
    //提交后的回调函数
    var callback = $form.attr("callback");
    if (callback) {
        callback = eval(callback);
    }
    if ($iframe.size() == 0) {
        $iframe = $("<iframe id='callbackframe' name='callbackframe' src='about:blank' style='display:none'></iframe>").appendTo("body");
    }
    form.target = "callbackframe";
    layer.msg($form.attr("loadingmsg")||'Loading', { icon: 16, time: 0, shade: [0.4, '#393D49'] });
    _iframeResponse($iframe[0], callback || TF.ajaxDone);
}
function _iframeResponse(iframe, callback) {
    var $iframe = $(iframe), $document = $(document);

    $iframe.bind("load", function (event) {
        $iframe.unbind("load");

        if (iframe.src == "javascript:'%3Chtml%3E%3C/html%3E';" || // For Safari
			iframe.src == "javascript:'<html></html>';") { // For FF, IE
            return;
        }

        var doc = iframe.contentDocument || iframe.document;

        // fixing Opera 9.26,10.00
        if (doc.readyState && doc.readyState != 'complete') return;
        // fixing Opera 9.64
        if (doc.body && doc.body.innerHTML == "false") return;

        var response;

        if (doc.XMLDocument) {
            // response is a xml document Internet Explorer property
            response = doc.XMLDocument;
        } else if (doc.body) {
            try {
                response = $iframe.contents().find("body").text();
                response = jQuery.parseJSON(response);
            } catch (e) { // response is html document or plain text
                response = doc.body.innerHTML;
            }
        } else {
            // response is a xml document
            response = doc;
        }
        callback(response);
    });
}
//validation后表单提交函数
function FormSubmitCallBack(form) {
    var $form = $(form);
    if ($form.attr("iframesubmit")) {
        iframeCallback(form);
        return true;
    }
    //表单提交前的自定义函数
    var beforeFn = $(form).attr("before");
    if (beforeFn) {
        beforeFn = eval(beforeFn);
        if (!beforeFn($(form))) {
            return false;
        }
    }
    //提交后的回调函数
    var callback = $form.attr("callback");
    if (callback) {
        callback = eval(callback);
    } else {
        callback = TF.ajaxDone;
    }
    var _submitFn = function () {
        layer.msg($form.attr("loadingmsg")||'Loading', { icon: 16, time: 0, shade: [0.4, '#393D49'] });
        $.ajax({
            type: 'Post',
            url: $form.attr("action"),
            data: $form.serializeArray(),
            dataType: "json",
            cache: false,
            success: callback,
            error: TF.ajaxError
        });
    }
    _submitFn();
    return false
}

$(function () {
    initUI($("body"));
    $("ul.selector").find("li:first").addClass("first");
});

function initUI($p) {
    $("a[rel='ajaxTodo']", $p).click(function () {
        var title = $(this).attr("title");
        var url = $(this).attr("href");
        if (title) {
            layer.msg(title, {
                time: 0 //不自动关闭
                , btn: ['确定', '取消']
                , shade: [0.4, '#393D49']
                , yes: function (index) {
                    layer.close(index);
                    TF.ajaxDo(url);
                }
            });
        } else {
            TF.ajaxDo(url);
        }
        return false;
    });
    $("body").on("click", "tr:not(.noclick)", function (event) {        
        //if ($(this).find(":checkbox").size() > 0) {
        //    var src = $(event.srcElement);
        //    if (!src.is("input")) {
        //        var cbbox = $(this).find(":checkbox");
        //        cbbox.attr("checked", !cbbox.attr("checked"));
        //    }
        //    $(this).toggleClass("highlight");
            
        //}
    })
}

function loadFrame(frameid, src, tip) {
    if (tip) {
        layer.msg(tip, { icon: 16, time: 0, shade: [0.4, '#393D49'] });
    }
    var ifm = $("#" + frameid);
    ifm.attr("height","0px");
    ifm.attr("src", src);
    ifm.bind("load", function (event) {
        layer.closeAll();
        setTimeout(function () {iFrameHeight(frameid); }, 200);
        
    });
}

function iFrameHeight(frameid) {
    var ifm = document.getElementById(frameid);
    var subWeb = document.frames ? document.frames[frameid].document : ifm.contentDocument;
    if (ifm != null && subWeb != null) {
        ifm.height = subWeb.body.scrollHeight;
        ifm.width = subWeb.body.scrollWidth;
    }
}

function exportAll() {
    var availabledate = $("#availabledate").val();
    if (!availabledate) {
        return false;
    } else {
        TF.loading("正在导出,请稍候");
        TF.ajaxDo("/Do.ashx?action=ExportAll&availabledate=" + availabledate, exportCallback);
    }
}


//设置需要设置数字样式的表头名
TF.initGrid = function ($table, $arr) {
    var tdindex_arr = [];
    if ($arr) { tdindex_arr = $arr; } else {
        var gridHeads = ["支出预算", "支出入账金额", "累计支出入账"];
        $table.find("th").each(function () {
            var indexVar = $.inArray($(this).html(), gridHeads);
            if (indexVar >= 0) {
                tdindex_arr.push($(this).index());
            }
        });
    }
    for (var i = 0; i < tdindex_arr.length; i++) {
        $table.find("tr").each(function () {
            $(this).find("td").eq(tdindex_arr[i]).addClass("digit");
        })

    }
}

//下载文件
function downloadFile(filepath) {
    window.CallCSharpMethod("DownloadFile", filepath);
    //$("#_hidden").attr("src", filepath);
}

function unique(arr) {
    var result = [], hash = {};
    for (var i = 0, elem; (elem = arr[i]) != null; i++) {
        if (!hash[elem]) {
            result.push(elem);
            hash[elem] = true;
        }
    }
    return result;
}

//序列化表单为对象
(function ($) {
    var serializeArray = $.fn.serializeArray;

    $.fn.serializeObj = function () {
        var arr = serializeArray.apply(this);
        var obj = {};
        for (var i = 0; i < arr.length; i++) {
            obj[arr[i].name] = arr[i].value;
        }
        return obj;
    };

})(jQuery);

function showPDF(filepath) {
    if (self == top) {
        //弹出框
        var pdfLayer = layer.open({
            type: 2,
            title: '查看PDF文件',
            closeBtn: 1,
            area: ['1000px', '600px'],
            shadeClose: false,
            content: "/Show/PDF?path=" + filepath
        });
        layer.full(pdfLayer);
    } else {
        top.showPDF(filepath);
    }
}


Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};