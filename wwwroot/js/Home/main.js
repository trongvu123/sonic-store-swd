/* Minification failed. Returning unminified contents.
(460,16-17): run-time error JS1014: Invalid character: `
(460,18-19): run-time error JS1004: Expected ';': {
(460,33-38): run-time error JS1004: Expected ';': trước
(460,38-39): run-time error JS1014: Invalid character: `
(461,7-11): run-time error JS1034: Unmatched 'else'; no 'if' defined: else
(463,16-17): run-time error JS1014: Invalid character: `
(463,18-19): run-time error JS1004: Expected ';': {
(463,33-38): run-time error JS1004: Expected ';': trước
(463,38-39): run-time error JS1014: Invalid character: `
(464,7-11): run-time error JS1034: Unmatched 'else'; no 'if' defined: else
(466,16-17): run-time error JS1014: Invalid character: `
(466,18-19): run-time error JS1004: Expected ';': {
(466,30-35): run-time error JS1004: Expected ';': trước
(466,35-36): run-time error JS1014: Invalid character: `
(467,7-11): run-time error JS1034: Unmatched 'else'; no 'if' defined: else
(469,16-17): run-time error JS1014: Invalid character: `
(469,18-19): run-time error JS1004: Expected ';': {
(469,30-35): run-time error JS1004: Expected ';': trước
(469,35-36): run-time error JS1014: Invalid character: `
(470,7-11): run-time error JS1034: Unmatched 'else'; no 'if' defined: else
(474,31-32): run-time error JS1014: Invalid character: `
(474,34-35): run-time error JS1004: Expected ';': {
(474,65-66): run-time error JS1195: Expected expression: :
(474,105-106): run-time error JS1004: Expected ';': {
(474,145-146): run-time error JS1004: Expected ';': $
(474,166-167): run-time error JS1014: Invalid character: `
(598,30-31): run-time error JS1100: Expected ',': =
(598,42-43): run-time error JS1002: Syntax error: ,
(598,54-55): run-time error JS1100: Expected ',': =
(808,23-24): run-time error JS1100: Expected ',': =
(808,34-35): run-time error JS1002: Syntax error: ,
(808,38-39): run-time error JS1100: Expected ',': =
(843,23-24): run-time error JS1100: Expected ',': =
(1088,21-22): run-time error JS1195: Expected expression: )
(1088,24-25): run-time error JS1195: Expected expression: >
(1090,10-11): run-time error JS1195: Expected expression: ,
(1134,1-2): run-time error JS1002: Syntax error: }
(1137,38-39): run-time error JS1004: Expected ';': {
(1150,2-3): run-time error JS1195: Expected expression: )
(1152,14-15): run-time error JS1195: Expected expression: )
(1152,16-17): run-time error JS1004: Expected ';': {
(1162,1-2): run-time error JS1195: Expected expression: )
(1163,24-25): run-time error JS1195: Expected expression: )
(1163,26-27): run-time error JS1004: Expected ';': {
(1179,67-68): run-time error JS1195: Expected expression: >
(1179,69-70): run-time error JS1014: Invalid character: `
(1179,94-95): run-time error JS1195: Expected expression: <
(1179,101-102): run-time error JS1014: Invalid character: `
(1179,103-104): run-time error JS1004: Expected ';': )
(1179,112-113): run-time error JS1197: Too many errors. The file might not be a JavaScript file: )
 */

function loadAjaxPost(url, params, option, type) {
    if (checkEmpty(type)) { type = 'progress'; }
    var _option = {
        beforeSend: function () { },
        success: function (result) { },
        error: function (error) { }
    }
    $.extend(_option, option);
    $.ajax({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        },
        type: 'POST',
        url: url,
        data: params,
        beforeSend: function () {
            switch (type) {
                case 'progress': activeProgress(0, 'open'); break;
                case 'loading': loadingBox('open'); break;
            }
            _option.beforeSend();
        },
        success: function (result) {
            switch (type) {
                case 'progress': activeProgress(99, 'close'); break;
                case 'loading': loadingBox('close'); break;
            }
            _option.success(result);
        },
        error: function (error) {
            /* Có lỗi sẽ ân Module Loading và thông báo */
            switch (type) {
                case 'progress': activeProgress(99, 'close'); break;
                case 'loading': loadingBox('close'); break;
            }
            alertText('Có lỗi xảy ra. Vui lòng thử lại!', 'error')
            _option.error(error);
        }
    })
}

function loadAjaxGet(url, option, type) {
    if (checkEmpty(type)) { type = 'progress'; }
    var _option = {
        beforeSend: function () { },
        success: function (result) { },
        error: function (error) { }
    }
    $.extend(_option, option);
    $.ajax({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        },
        type: 'GET',
        url: url,
        beforeSend: function () {
            switch (type) {
                case 'progress': activeProgress(0, 'open'); break;
                case 'loading': loadingBox('open'); break;
            }
            _option.beforeSend();
        },
        success: function (result) {
            switch (type) {
                case 'progress': activeProgress(99, 'close'); break;
                case 'loading': loadingBox('close'); break;
            }
            _option.success(result);
        },
        error: function (error) {
            /* Có lỗi sẽ ân Module Loading và thông báo */
            switch (type) {
                case 'progress': activeProgress(99, 'close'); break;
                case 'loading': loadingBox('close'); break;
            }
            alertText('Có lỗi xảy ra. Vui lòng thử lại!', 'error')
            _option.error(error);
        }
    })
}
// LoadingBox
function loadingBox(type) {
    if (type == 'open') {
        $('body').append('<section id="loading_box"><div id="loading_image"></div></section>');
        $("#loading_box").css({ visibility: "visible", opacity: 0.0 }).animate({ opacity: 1.0 }, 200);
    } else {
        $("#loading_box").animate({ opacity: 0.0 }, 200, function () {
            $(this).remove();
        });
    }
}
// LoadProgessBar
var progress = null;
function activeProgress(number, type) {
    if (type == 'open') {
        $('body').append('<section class="progress-box"><div class="progress-run"></div></section>');
    }
    clearInterval(progress);
    $('.progress-box').css('display', 'block');
    $('.progress-run').css('width', number + '%');
    progress = setInterval(function () {
        if (number <= 100) {
            $('.progress-run').css('width', number + '%');
            number = number + 1;
        } else {
            clearInterval(progress);
        }
    }, 100);
    if (type == 'close') {
        setTimeout(function () {
            $('.progress-box').remove();
        }, 1000);
    }
}
function alertText(text, type) {
    switch (type) {
        case 'success':

            break;
        case 'info':

            break;
        case 'error':

            break;
        case 'warning':

            break;
    }
}

// check định dạng email
function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

// check định dạng số điện thoại
function validatePhone(phone) {
    var flag = false;
    phone = phone.trim();
    phone = phone.replace('(+84)', '0');
    phone = phone.replace('+84', '0');
    phone = phone.replace('0084', '0');
    phone = phone.replace(/ /g, '');
    if (phone != '') {
        if (phone.length >= 9 && phone.length <= 11) {
            flag = true;
        } else {
            flag = false;
        }
    }
    return flag;
}

// thêm cookie
function setCookie(cName, cValue, expDays) {
    let date = new Date();
    date.setTime(date.getTime() + (expDays * 24 * 60 * 60 * 1000));
    const expires = "expires=" + date.toUTCString();
    document.cookie = cName + "=" + cValue + "; " + expires + "; path=/";
}

function setCookieWithPath(key, path, value) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (day * 24 * 60 * 60 * 1000));
    document.cookie = key + '=' + value + ';path=' + path + ';expires=' + expires.toUTCString();
}
// lấy cookie
function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// Xóa cookie
function deleteCookie(key, path) {
    var expires = new Date();
    expires.setTime(expires.getTime() - 1);
    document.cookie = key + '=; path=' + path + '; expires=' + expires.toUTCString();
}

/**
 * rewrite url: thêm trên url không load lại trang
 * @param string            url_page: link dùng để cập nhật lên thanh url đã qua xử lý
 */
function update_url(url_page) {
    history.pushState(null, null, url_page);
}
// truyền param lên url
// param_obj: là một obj có dạng {key:value,key1:value2}
function pushOrUpdate(param_obj) {
    var url = new URL(window.location.href);
    $.each(param_obj, function (key, value) {
        url.searchParams.set(key, value);
    })
    var newUrl = url.href;
    update_url(newUrl);
}

// Lấy gái trị param tại Url
function getUrlParameter(url, name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(url);
    if (results == null) {
        return null;
    }
    return decodeURI(results[1]) || null;
}

// Check giá trị tồn tại trong mảng
function checkValueInArray(value, arr) {
    var status = false;
    for (var i = 0; i < arr.length; i++) {
        var name = arr[i];
        if (name == value) {
            status = true;
            break;
        }
    }
    return status;
}

// Trả về true nếu rỗng
function checkEmpty(value) {
    if (value == null) {
        return true;
    } else if (value == 'null') {
        return true;
    } else if (value == undefined) {
        return true;
    } else if (value == '') {
        return true;
    } else {
        return false;
    }
}

// format định dạng kích thước
function formatSizeUnits(bytes) {
    if (bytes >= 1073741824) { bytes = (bytes / 1073741824).toFixed(0) + "GB"; }
    else if (bytes >= 1048576) { bytes = (bytes / 1048576).toFixed(0) + "MB"; }
    else if (bytes >= 1024) { bytes = (bytes / 1024).toFixed(0) + "KB"; }
    else if (bytes > 1) { bytes = bytes + " bytes"; }
    else if (bytes == 1) { bytes = bytes + " byte"; }
    else { bytes = "0 bytes"; }
    return bytes;
}

// lưu html 1 thẻ vào clipboard và copy
// Lấy copy trên iphone, ipad
window.Clipboard = (function (window, document, navigator) {
    var textArea,
        copy;
    function isOS() {
        return navigator.userAgent.match(/ipad|iphone/i);
    }
    function createTextArea(text) {
        textArea = document.createElement('textArea');
        textArea.value = text;
        document.body.appendChild(textArea);
    }
    function selectText() {
        var range,
            selection;
        if (isOS()) {
            range = document.createRange();
            range.selectNodeContents(textArea);
            selection = window.getSelection();
            selection.removeAllRanges();
            selection.addRange(range);
            textArea.setSelectionRange(0, 999999);
        } else {
            textArea.select();
        }
    }
    function copyToClipboard() {
        document.execCommand('copy');
        document.body.removeChild(textArea);
    }
    copy = function (text) {
        createTextArea(text);
        selectText();
        copyToClipboard();
    };
    return {
        copy: copy
    };
})(window, document, navigator);
// Copy text
function copyText(text) {
    $('body').append('<input type="text" id="copy-input" value="' + text + '" style="opacity:0;height:0;width:5px;position:absolute;top:-1px;left:-1px;">');
    if (document.execCommand('copy')) {
        $('#copy-input').select();
        document.execCommand("copy");
    } else {
        Clipboard.copy(text);
    }
    $('#copy-input').remove();
}

// Chuyển chuỗi sang dạng slug
function convertToSlug(str) {
    //Đổi chữ hoa thành chữ thường
    slug = str.toLowerCase();
    //Đổi ký tự có dấu thành không dấu
    slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
    slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
    slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
    slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
    slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
    slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
    slug = slug.replace(/đ/gi, 'd');
    //Xóa các ký tự đặt biệt
    slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\'|\"|\:|\;|_/gi, '');
    //Đổi khoảng trắng thành ký tự gạch ngang
    slug = slug.replace(/ /gi, "-");
    //Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
    //Phòng trường hợp người nhập vào quá nhiều ký tự trắng
    slug = slug.replace(/\-\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-/gi, '-');
    slug = slug.replace(/\-\-/gi, '-');
    //Xóa các ký tự gạch ngang ở đầu và cuối
    slug = '@' + slug + '@';
    slug = slug.replace(/\@\-|\-\@|\@/gi, '');
    //In slug ra textbox có id “slug”
    return slug
}

// Tabs FadeIn
// menu: class của menu có cấu trúc ul[class=menu]>li>a[href="#content_id_name"]
// content: class của thẻ bao nội dung VD: div[class=content,id=content_id_name]
// active: tên class khi active content
function tabs(menu, content, active) {
    // Ẩn toàn bộ nội dung trong content
    $("." + content).hide();
    // Hiển thị và đánh active cho thẻ li và content đầu tiên
    $("ul." + menu + " li:first").addClass('active').fadeIn();
    $("." + content + ":first").fadeIn();
    // khi thẻ li được click
    $("ul." + menu + " li").on('click', function (e) {
        e.preventDefault();
        // bỏ toàn bộ active cho các thẻ li trước đó
        $("ul." + menu + " li").removeClass('active');
        // Đánh lại active cho thẻ li này
        $(this).addClass('active');
        var activeTab = $(this).find('a').attr('href');
        $("." + content).hide();
        $(activeTab).fadeIn();
    });
}

// Định dạng giá
function format_price(number) {
    if (number == 0) {
        return 'Miễn phí';
    } else {
        number += '';
        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + '.' + '$2');
        }
        number = x1 + x2 + "đ";
        return number;
    }
}

/*
 * Check hình ảnh có tồn tại hay không
 * Nếu không tốn tại thì trả về ảnh gốc
 */
function notFoundImage() {
    $('img').each(function (index, item) {
        element = $(this);
        if (!checkEmpty($(this).attr('data-original'))) {
            src = element.data('original');
            if (!checkEmpty(src)) {
                var tester = new Image();
                tester.src = src;
                tester.onerror = function () {
                    $(item).attr('data-original', '/assets/img/default_image.png').data('original', '/assets/img/default_image.png');
                    lazyloadImage();
                }
            }
        } else {
            src = element.attr('src');
            if (!checkEmpty(src)) {
                var tester = new Image();
                tester.src = src;
                tester.onerror = function () {
                    $(item).attr('src', '/assets/img/default_image.png');
                }
            }
        }
    });
}

/*
 * Lazyload cho hình ảnh
 * Chú ý: ảnh phải được đặt trong thẻ img và có class là lazyload
 */
function lazyloadImage() {
    if ($.isFunction($.fn.lazyload)) {
        $('.lazyload').lazyload({
            threshold: 100,
            failure_limit: 10
        });
    }
}
function formatPrice(price) {

    if (price == 0) {
        return 'Liên hệ'
    } else {
        var numericPrice = parseFloat(price);
        var formattedPrice = numericPrice.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        return formattedPrice + " ₫";
    }

}


function formatPhoneNumber(str) {

    var result = str;

    let cleaned = ("" + str).replace(/\D/g, "");
    let match = cleaned.match(/^(\d{4})(\d{3})(\d{3})$/);

    if (match) {
        result = match[1] + "." + match[2] + "." + match[3];
    }
    return result;
}



function getTimeAgo(timestamp) {

    const currentTimestamp = new Date().getTime();
    const targetTimestamp = new Date(timestamp).getTime();
    const elapsedMilliseconds = currentTimestamp - targetTimestamp;

    if (elapsedMilliseconds < 60000) {
        const seconds = Math.floor(elapsedMilliseconds / 1000);
        return `${seconds} giây trước`;
    } else if (elapsedMilliseconds < 3600000) {
        const minutes = Math.floor(elapsedMilliseconds / 60000);
        return `${minutes} phút trước`;
    } else if (elapsedMilliseconds < 86400000) {
        const hours = Math.floor(elapsedMilliseconds / 3600000);
        return `${hours} giờ trước`;
    } else if (elapsedMilliseconds < 2592000000) { // 30 ngày
        const days = Math.floor(elapsedMilliseconds / 86400000);
        return `${days} ngày trước`;
    } else {
        const date = new Date(targetTimestamp);
        const minute = date.getMinutes();
        const hour = date.getHours();
        const formattedDate = ` ${hour < 10 ? '0' + hour : hour}:${minute < 10 ? '0' + minute : minute} - ${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
        return formattedDate;
    }
}
;

var page = 1;

function delay(t, e) {
    var i = 0;
    return function () {
        var a = this, s = arguments;
        clearTimeout(i),
            i = setTimeout((function () {
                t.apply(a, s)
            }
            ), e || 0)
    }
}

function loadUrl(key, value) {

    // Nếu giá trị là "select", lấy giá trị từ một phần tử <select>
    if (value === "select") {
        value = $('select[name="sort"]').val();
    }

    var currentUrl = window.location.href;
    var keyValue = key + "=" + value;
    var tempUrl = "";
    var newUrl = "";

    // Xử lý trường hợp URL chứa "&page"
    if (currentUrl.includes("&page")) {
        currentUrl = decodeURI(currentUrl.split("&page")[0]);
    } else if (currentUrl.includes("?page")) {
        currentUrl = decodeURI(currentUrl.split("?page")[0]);
    }

    // Kiểm tra xem URL có chứa phần tử key không
    if (currentUrl.includes(key)) {
        // Xử lý các điều kiện phức tạp
        if (
            currentUrl.indexOf(keyValue) === -1 ||
            (key !== "wattage" &&
                key !== "category" &&
                key !== "sort" &&
                !key.includes("filter") &&
                key !== "price_range[]" &&
                key !== "price[]" &&
                key !== "brand[]")
        ) {
            // Xử lý các trường hợp đặc biệt và cập nhật newUrl
            if (key.includes("filter")) {
                $(".fil_" + value).toggleClass("active");
            }
            if (key === "brand[]") {
                $(".brand_" + value).toggleClass("active");
            }
            if (key === "price[]" || key === "price_range[]") {
                $(".price_" + value).toggleClass("active");
            }
            if (key === "sort" || key === "category" || key === "wattage") {
                if (key === "sort") {
                    $(".order_" + value).toggleClass("active");
                } else {
                    $("." + key + "_" + value).toggleClass("active");
                    var tempUrlArray = currentUrl.split(key);
                    var o = tempUrlArray[1].split("&");

                    if (o[1] !== null) {
                        var r = o.shift();
                        r = o.join("&");
                        newUrl = tempUrlArray[0] + keyValue + "&" + r;
                    } else {
                        newUrl = tempUrlArray[0] + keyValue;
                    }
                }
            } else {
                newUrl =
                    currentUrl.lastIndexOf("?") !== -1
                        ? currentUrl + "&" + keyValue
                        : currentUrl + "?" + keyValue;
            }
        } else {
            // Xử lý trường hợp loại bỏ key và cập nhật newUrl
            tempUrl = currentUrl
                .replace("?" + keyValue + "&", "?")
                .replace("&" + keyValue + "&", "&")
                .replace("&" + keyValue, "")
                .replace("?" + keyValue, "");

            if (key.includes("filter")) {
                $(".fil_" + value).removeClass("active");
            }
            if (key === "brand[]") {
                $(".brand_" + value).removeClass("active");
            }
            if (key === "price[]" || key === "price_range[]") {
                $(".price_" + value).removeClass("active");
            }
            newUrl = tempUrl;
        }
    } else {
        // Xử lý trường hợp thêm key và cập nhật newUrl
        if (key.includes("filter")) {
            $(".fil_" + value).addClass("active");
        }
        if (key === "brand[]") {
            $(".brand_" + value).addClass("active");
        }
        if (key === "price[]" || key === "price_range[]") {
            $(".price_" + value).addClass("active");
        }
        newUrl =
            currentUrl.lastIndexOf("?") !== -1
                ? currentUrl + "&" + keyValue
                : currentUrl + "?" + keyValue;
    }

    // Gọi hàm update_url để cập nhật URL
    update_url(newUrl);
}

function loadData(actionType = "progress", emptyFlag = "") {
    loadAjaxGetPaginate(window.location.href, {
        beforeSend: function () {
            // Add any necessary code before sending the request
        },
        success: function (responseData) {
            // Update filter elements
            $(".filter_sort").remove();
            $(".filter-list").append(responseData.filter);

            // Handle different cases based on 'emptyFlag'
            if (emptyFlag === "empty") {
                $(".filter-item").removeClass("active");
                $("#listdata").empty();
                $("#listdata").html(responseData.html);
                let height = $("#listdata").offset().top - 100;
                $("html, body").animate({
                    scrollTop: height
                }, "slow");
            } else {

                $("#listdata").append(responseData.html);

                if (responseData.html == "") {
                    $(".section-product__content .btn-main").remove();
                }

            }

            // Update filter result section
            if (responseData.html_filter_result !== "") {
                $(".filter-result").removeClass("hide");
                $(".filter-result").empty();
                $(".filter-result").html(responseData.html_filter_result);
            } else {
                $(".filter-result").addClass("hide");
                $(".filter-result").empty();
            }

            // Update class 'show' or 'hide' based on filter items
            $(".list-filter .section-list__item").removeClass("active");
            $(".list-filter .section-list__item").each(function () {
                if ($(this).find("li.active").length === 0) {
                    $(this).removeClass("show");
                } else {
                    $(this).addClass("show");
                }
            });

            // Update pagination and show buttons
            $(".pagination").empty();
            $(".pagination").html(responseData.pagination);
            $(".section-product__content .btn-main").show();

            // Format prices
            $(".js-format-price").each(function () {
                var price = $(this).attr("data-price");
                $(this).html(formatPrice(price));
            });

            // Calculate and display sale percentage
            $(".ex_pricesale ").each(function () {
                var price = $(this).attr("data-price");
                var price1 = $(this).attr("data-price1");

                if (price1 > price) {
                    $(this).removeClass('d-none');
                    $(this).html(100 - (Math.round((price / price1) * 100)) + '%');
                }
            });
        },
        error: function (errorData) {
            // Handle error if needed
        }
    }, actionType);
}

function loadAjaxGetPaginate(url, options, loadingBoxType) {
    // Kiểm tra nếu loadingBoxType trống, gán giá trị mặc định là "progress"
    if (checkEmpty(loadingBoxType)) {
        loadingBoxType = "progress";
    }

    // Mặc định các tham số cho Ajax request
    var ajaxOptions = {
        beforeSend: function () { },
        success: function (data) { },
        error: function (error) { }
    };

    // Mở rộng các tùy chọn của người dùng vào ajaxOptions
    $.extend(ajaxOptions, options);

    // Thực hiện Ajax request
    $.ajax({
        headers: {
            "X-CSRF-TOKEN": $('meta[name="csrf-token"]').attr("content")
        },
        type: "POST",
        url: url,
        beforeSend: function () {
            // Hiển thị hộp loadingBox trước khi thực hiện Ajax request
            loadingBox("open");
        },
        success: function (data) {
            // Sau khi thành công, đóng hộp loadingBox sau 1 giây và gọi hàm success của người dùng
            setTimeout(function () {
                loadingBox("close");
            }, 1000);
            ajaxOptions.success(data);
        },
        error: function (error) {
            // Đóng hộp loadingBox, hiển thị thông báo lỗi và gọi hàm error của người dùng
            loadingBox("close");
            alert("Có lỗi xảy ra. Vui lòng thử lại!", "error");
            ajaxOptions.error(error);
        }
    });
}

var selectedText = $(".location-select option:selected").html();
$(".my_location").text(selectedText);


function validForm(formId) {

    var isValid = true;

    $("#" + formId + " .form-control.required").each(function () {

        var input = $(this);
        var name = input.attr("name");

        $(this).css("border", "1px solid #ccc").parent().find(".err_show").removeClass("active");

        if (
            (name === "phone" || name === "ship_phone") &&
            (input.val() === "" || !isPhone(input.val()))
        ) {
            handleValidationFailure(input, "phone");
            isValid = false;
        } else if (
            (name === "product_name" && input.val() === "0")
        ) {
            handleValidationFailure(input);
            isValid = false;
        } else if (
            (name === "store_id" && input.val() === "0")
        ) {
            handleValidationFailure(input);
            isValid = false;
        } else if (
            (name === "province_id" && input.val() === "0")
        ) {
            handleValidationFailure(input);
            isValid = false;
        } else if (
            (name === "ship_province_id" && input.val() === "0")
        ) {
            handleValidationFailure(input);
            isValid = false;
        } else if ((formId === "address-default" || formId === "ship-address" || formId === "installment-card") && input.val() === "") {
            handleValidationFailure(input, "null");
            isValid = false;
        }

    });

    if (!isValid) {
        var adjustedFormId = formId;
        if (adjustedFormId === "formComment" || adjustedFormId.includes("formComment")) {
            adjustedFormId = "cmt_vote";
        }
    }

    return isValid;
}

function handleValidationFailure(input, errorType) {

    input.parent().find(".err_show" + (errorType ? "." + errorType : "")).addClass("active");

    input.css({
        border: "2px solid #dc1f26",
        visibility: "visible"
    });

    input.focus();

    input.keyup(function () {
        input.css("border", "1px solid #ccc").parent().find(".err_show").removeClass("active");
    })

    input.change(function () {
        input.css("border", "1px solid #ccc").parent().find(".err_show").removeClass("active");
    })

}

function handleFocus() {
    $(this).css("border", "1px solid #ccc").parent().find(".err_show").removeClass("active");
}

function isEmail(t) {
    return /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(t)
}
function isPhone(t) {
    return /((09|03|07|08|05)+([0-9]{8})\b)/g.test(t)
}

function alert_show(t = "success", e = "") {
    $("#toast-container .toast").addClass("toast-" + t),
        $("#toast-container .toast div").html(e),
        $("#toast-container").css("display", "block"),
        setTimeout((function () {
            $("#toast-container").css("display", "none"),
                $("#toast-container .toast").removeClass("toast-" + t),
                $("#toast-container .toast div").text("")
        }
        ), 5e3)
}

function update_url(t) {
    history.pushState(null, null, t)
}

function pushOrUpdate(parameters) {
    // Tạo một đối tượng URL từ địa chỉ URL hiện tại
    var urlObject = new URL(window.location.href);

    // Duyệt qua mảng các tham số và giá trị, đặt chúng vào đối tượng URL
    $.each(parameters, function (key, value) {
        urlObject.searchParams.set(key, value);
    });

    // Cập nhật URL với các tham số mới và gọi hàm update_url
    update_url(urlObject.href);
}


function getUrlParameter(t, e) {
    var i = new RegExp("[?&]" + e + "=([^&#]*)").exec(t);
    return null == i ? null : decodeURI(i[1]) || null
}

function loadingBox(t = "open") {
    "open" == t ? $("#loading_box").css({
        visibility: "visible",
        opacity: 0
    }).animate({
        opacity: 1
    }, 200) : $("#loading_box").animate({
        opacity: 0
    }, 200, (function () {
        $("#loading_box").css("visibility", "hidden")
    }
    ))
}
function checkEmpty(t) {
    return null == t || ("null" == t || (null == t || ("" == t || t == [])))
}

function removeFilter(filterType, filterValue) {
    // Lấy địa chỉ URL hiện tại và tách thành domain và query string
    var urlParts = decodeURI(window.location.href).split("?");
    var domain = urlParts[0];
    var queryString = urlParts[1];

    // Kiểm tra xem có query string hay không
    if (queryString !== "") {
        // Xử lý trường hợp "all"
        if (filterType === "all") {
            update_url(domain); // Cập nhật URL
            loadData("progress", "empty"); // Tải dữ liệu mới
            // Loại bỏ lớp "active" từ tất cả các phần tử
            $(".section-list__item ul li").removeClass("active");
            // Ẩn tất cả các phần tử
            $(".list-sort .section-list__item").removeClass("show");
        } else {
            // Loại bỏ lớp "active" từ các phần tử cụ thể
            $(".filter-" + filterValue + " ul li").removeClass("active");
        }

        // Nếu query string chứa tham số filterType, loại bỏ nó
        if (queryString.lastIndexOf(filterType) !== -1) {
            var queryArray = queryString.split("&");
            var newQueryString = queryArray.filter(function (item) {
                if (!item.includes(filterType)) {
                    return item;
                }
            });

            newQueryString = newQueryString.join("&");

            // Cập nhật URL với query string mới hoặc không có query string nếu rỗng
            if (newQueryString !== "") {
                update_url(domain + "?" + newQueryString);
            } else {
                update_url(domain);
            }

            // Tải dữ liệu mới
            loadData("progress", "empty");
        }
    }

    // Kiểm tra và điều chỉnh lớp "show" cho các phần tử
    $(".list-filter .section-list__item").each(function () {
        if ($(this).find("li.active").length === 0) {
            $(this).removeClass("show");
        } else {
            $(this).addClass("show");
        }
    });
}


function tabs(t, e, i) {
    $("ul." + t + " li:first a").addClass("active").fadeIn(),
        $("." + e + ":first").addClass(i),
        $("ul." + t + " li").on("click", (function (i) {
            i.preventDefault(),
                $("ul." + t + " li a").removeClass("active"),
                $(this).find("a").addClass("active");
            var a = $(this).find("a").attr("href");
            $("." + e).removeClass("active"),
                $(a).addClass("active")
        }
        ))
}
sudoSlide("slideList", [{
    maxWidth: 99999999999999,
    minWidth: 0,
    qtyItem: 1,
    nextItem: 1
}], !1, !0, !0, 5e3, 1, 0, 1, "data-thumnail"),
    sudoSlide("thirdSlide", [{
        maxWidth: 99999999999999,
        minWidth: 992,
        qtyItem: 5,
        nextItem: 1
    }, {
        maxWidth: 992,
        minWidth: 768,
        qtyItem: 4,
        nextItem: 1
    }, {
        maxWidth: 768,
        minWidth: 480,
        qtyItem: 3,
        nextItem: 1
    }, {
        maxWidth: 480,
        minWidth: 0,
        qtyItem: 2,
        nextItem: 1
    }], !0, !1, !1, 5e3, 1, 10, 0, 1),
    sudoSlide("product-slide", [{
        maxWidth: 768,
        minWidth: 430,
        qtyItem: 6,
        nextItem: 1.2
    }, {
        maxWidth: 430,
        minWidth: 375,
        qtyItem: 5,
        nextItem: 1.2
    }, {
        maxWidth: 375,
        minWidth: 0,
        qtyItem: 3,
        nextItem: 1.2
    }], !1, !1, !1, 5e3, 0, 10, 0, 0),
    $.ajaxSetup({
        headers: {
            "X-CSRF-TOKEN": $('meta[name="csrf-token"]').attr("content")
        }
    });

$(document).ready(function () {

    Fancybox.bind("[data-fancybox='showmap_footer']", {
        Hash: false,
        Thumbs: {
            type: "classic",
        }
    });


    if ($("body .css-content iframe").length) {
        setTimeout(function () {
            // Thiết lập thuộc tính src cho mỗi iframe và thêm lớp text-center cho phụ huynh của iframe
            $("body .css-content iframe").each(function () {
                let src = $(this).data("src");
                $(this).attr("src", src);
                $(this).parent().addClass("text-center");
            });
        }, 5000);
    }

    // Xử lý hover trên icon breadcrumb
    $(".header-breadcrumb__icon").hover(
        function (event) {
            event.preventDefault();
            $(this).parents(".header-breadcrumb").addClass("active");
        },
        function () {
            $(this).parents(".header-breadcrumb").removeClass("active");
        }
    );

    // Xử lý hover trên menu category breadcrumb
    $(".header-breadcrumb .menu-category").hover(
        function (event) {
            event.preventDefault();
            $(this).parents(".header-breadcrumb").addClass("active");
        },
        function () {
            $(this).parents(".header-breadcrumb").removeClass("active");
        }
    );

    // Xử lý click vào nút "result" trong btn-filter
    $("body").on("click", ".btn-filter .result", function () {
        loadData("progress", "empty");
        $(".fixed").removeClass("active");
    });

    // Xử lý click vào các mục trong list-sort
    $("body").on("click", ".list-sort .section-list__item", function () {
        loadData("progress", "empty");
        $(".fixed").removeClass("active");
        $(this).toggleClass("show");
        $(this).siblings().removeClass("show");
    });

    // Xử lý click vào trang kế tiếp trong section-product
    $("body").on("click", ".section-product .pagination a[rel=next]", function (event) {
        event.preventDefault();

        var nextPage = getUrlParameter($(this).attr("href"), "page");

        pushOrUpdate({ page: nextPage });

        loadData("progress", "append");
    });

    // Xử lý click vào trang kế tiếp trong post-page
    $("body").on("click", ".post-page .pagination a[rel=next]", function (event) {
        event.preventDefault();
        pushOrUpdate({ page: getUrlParameter($(this).attr("href"), "page") });
        loadData("progress", "empty");
    });

    // Xử lý click vào nút "btn-main span" trong section-product__content
    $("body").on("click", ".section-product__content .btn-main span", function () {
        if ($('.section-product__content .pagination a[rel=next]').length === 1) {
            $('.section-product__content .pagination a[rel=next]').trigger("click");
        } else {
            loadingBox("open");
            setTimeout(function () {
                loadingBox("close");
                $(".section-product__content .btn-main").hide();
            });
        }
    });


    var showLocationSuggest = getCookie("showLocationSuggest");

    if (showLocationSuggest == "") {

        setTimeout(function () {

            $.ajax({
                type: "POST",
                url: "/Ajax/SaveRegionOfClient",
                success: function () {
                    setCookie("showLocationSuggest", 1, 365)
                }
            });

        }, 2000)

    }


    var _isShowProvince = getCookie("_isShowProvince");

    if (_isShowProvince == "") {
        setTimeout(() => {
            $("#popUpChangeProvince").addClass("is-active");
        }, 5000)
    }


    // Hàm lọc tỉnh thành theo từ khóa tìm kiếm

    $('#searchProvinceInput').on('input', function () {
        var value = $(this).val().toLowerCase();
        $("#provinceList li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#provinceList li").on("click", function () {
        var _location = $(this).data("province-id");
        $.ajax({
            type: "POST",
            data: { cvalue: _location },
            url: "/ajax/setcookie",
            success: function () {
                setCookie("_isShowProvince", 1, 365)
                location.reload(true);
            }
        });
    })


    // Xử lý click vào nút "button" trong popUpChangeProvince
    $("body").on("click", "#popUpChangeProvince .button", function () {
        $("#popUpChangeProvince").removeClass("is-active");
    });

    // Xử lý sự kiện thay đổi trong location-select
    $(".location-select").change(function () {
        // Gửi yêu cầu AJAX để thiết lập cookie với giá trị mới và tải lại trang
        $.ajax({
            type: "POST",
            data: { cvalue: $(this).val() },
            url: "/ajax/setcookie",
            success: function () {
                location.reload(true);
            }
        });
    });
});


$(document).mouseup(function (event) {
    var suggestElements = $(".suggest");

    if (!suggestElements.is(event.target) && suggestElements.has(event.target).length === 0) {
        suggestElements.removeClass("show");
    };

    var suggestElements2 = $(".suggest-search");

    if (!suggestElements2.is(event.target) && suggestElements2.has(event.target).length === 0) {
        suggestElements2.removeClass("active");
    }

});

$((function () {
    $("#region-menu li").click((function () {
        const t = $(this).data("tab");
        $(".tab-pane").removeClass("active"),
            $(t).addClass("active"),
            $("#region-menu li").removeClass("active"),
            $(this).addClass("active")
    }
    ))
}
));
setInterval((function () {
    $(".countdown-data").each(function () {
        var t = $(this).data("end")
            , e = ($(this).data("gmt"),
                (new Date).getTime())
            , i = t - (e = Math.floor(e / 1e3))
            , a = n = r = o = 0;
        if (i > 0) {
            var s = parseInt(i, 10)
                , n = (a = Math.floor(s / 86400),
                    Math.floor(s / 3600) % 24);
            if ((o = s % 60) < 10)
                var o = "0" + o;
            if ((r = Math.floor(s / 60) % 60) < 10)
                var r = "0" + r
        }
        $(this).find(".days").html(a.toString().split("").map((t => `<span class="digit">${t}</span>`)).join("")),
            $(this).find(".hours").html(n.toString().split("").map((t => `<span class="digit">${t}</span>`)).join("")),
            $(this).find(".minutes").html(r.toString().split("").map((t => `<span class="digit">${t}</span>`)).join("")),
            $(this).find(".seconds").html(o.toString().split("").map((t => `<span class="digit">${t}</span>`)).join(""))
    }
    )
}
), 1e3);
$(document).ready((function () {
    var t = $(".top-left .menu-category").height()
        , e = $(".top-right").height()
        , i = $(".banner").height();
    t >= 40 && !e && $(".banner").css({
        height: t + i
    })
}
));

var suggest = null;
$("body").on("keyup", ".header-top .search input", delay((function () {
    var t = $(this).val()
        , e = {
            keyword: t
        };
    if (!t.length)
        return $(".suggest").removeClass("show"),
            !1;
    $(".loading-suggest").css("opacity", "1"),
        $.ajax({
            type: "POST",
            data: e,
            url: "/ajaxs/search.aspx",
            success: function (t) {
                $(".suggest").html(t).addClass("show");
                $('.header-overlay').addClass('active');
                $('.suggest-search').removeClass('active');
            }
        })
}
), 500));
var page = 1;
function sudoSlide(t = "", e = [], i = !0, a = !0, s = !0, n = 0, o = 0, r = 0, l = 0, c = "", d = []) {
    const u = document.querySelector(`#${t}`);
    if (!u)
        return !1;
    u.querySelectorAll("img").forEach((t => {
        t.addEventListener("dragstart", (t => {
            t.preventDefault()
        }
        ))
    }
    ));
    var h = document.querySelector(`#${t} .s-content`);
    if (h) {
        h.style.transform = "translateX(0px)";
        var p = document.querySelector(`#${t}`).parentNode
            , f = (window.getComputedStyle(p),
                p.clientWidth);
        if ("thirdSlide" == t)
            if ((f = p.clientWidth - 40) < 992)
                f = p.clientWidth - 20;
        var m = h.children
            , $ = 0
            , v = 0
            , g = 0;
        e.forEach((function (e, d) {
            let p = e.maxWidth
                , y = e.minWidth;
            if (f <= p && f > y) {
                if (v = e.qtyItem,
                    o = e.nextI || o,
                    $ = f / v * m.length + r * (v - 1) * m.length / (2 * v),
                    h.style.width = `${$}px`,
                    Array.from(m).forEach((t => {
                        t.style.width = $ / m.length - r * (v - 1) / v + "px",
                            t.style.marginRight = `${r}px`
                    }
                    )),
                    i) {
                    u.insertAdjacentHTML("beforeend", '<div class="nav"><div class="nav-next"></div><div class="nav-prev"></div></div>');
                    var _ = u.querySelector(".nav-next")
                        , b = u.querySelector(".nav-prev");
                    [_, b].forEach((t => {
                        t.addEventListener("click", (function () {
                            t === _ ? w(o || 1) : C(o || 1),
                                s && (clearInterval(g),
                                    S())
                        }
                        ), {
                            passive: !0
                        })
                    }
                    ))
                }
                if (a) {
                    let A = '<div class="dots">';
                    l && (A = `<div class="dots-custom"><div class="s-wrap" id="dot-custom-${t}"><div class="s-content">`);
                    for (let X = 0; X <= Math.round((m.length - v) / o); X++) {
                        let B = "";
                        l && (B = `\n                                <div class="dot-custom">\n                                    <p>${m[X].getAttribute(c)}</p>\n                                </div>\n                            `),
                            A += `<div class="dot dot-${X} ${0 == X ? "active" : ""}">${B}</div>`
                    }
                    A += "</div>",
                        l && (A += "</div></div>"),
                        u.insertAdjacentHTML("beforeend", A);
                    let D = document.querySelector(`#${t} .dot.active`)
                        , L = document.querySelectorAll(`#${t} .dot`);
                    const P = () => {
                        D.classList.remove("active"),
                            D = document.querySelector(`#${t} .dot-${x}`),
                            D.classList.add("active")
                    }
                        ;
                    for (let M of L)
                        M.addEventListener("click", (function (t) {
                            var e = t.target;
                            dotClass = e.getAttribute("class"),
                                dotIndex = dotClass.replace(/[^\d]+/g, ""),
                                dotIndex != x && (x = dotIndex - 1,
                                    w(o)),
                                P()
                        }
                        ), {
                            passive: !0
                        })
                }
                var x = 0;
                function C(e) {
                    x = Math.max(0, x - 1);
                    let i = $ / m.length * x * e;
                    if (h.style.transform = `translateX(${-i}px)`,
                        a) {
                        let e = document.querySelector(`#${t} .dot.active`);
                        document.querySelectorAll(`#${t} .dot`);
                        e.classList.remove("active"),
                            e = document.querySelector(`#${t} .dot-${x}`),
                            e.classList.add("active")
                    }
                    s && (clearInterval(g),
                        S())
                }
                function w(e) {
                    const i = Math.round((m.length - v) / e);
                    x = x >= i ? 0 : x + 1;
                    const n = $ / m.length * Math.min(x * e, (m.length - v) / e);
                    if (h.style.transform = `translateX(${-n}px)`,
                        a) {
                        let e = document.querySelector(`#${t} .dot.active`);
                        document.querySelectorAll(`#${t} .dot`);
                        e.classList.remove("active"),
                            e = document.querySelector(`#${t} .dot-${x}`),
                            e.classList.add("active")
                    }
                    s && (clearInterval(g),
                        S())
                }
                function S() {
                    g = setInterval((() => w(o || 1)), n)
                }
                s && S();
                let I, T, k, q = !1;
                const W = t => {
                    if (2 === t.button)
                        return !1;
                    I = "mousedown" === t.type ? t.pageX : t.changedTouches[t.changedTouches.length - 1].pageX,
                        q = !0
                }
                    , E = t => {
                        if (2 === t.button)
                            return !1;
                        if (T = "mouseup" === t.type ? t.pageX : t.changedTouches[t.changedTouches.length - 1].pageX,
                            q = !1,
                            checkHref = t.target,
                            I < T)
                            C(o || 1);
                        else if (I > T)
                            w(o || 1);
                        else {
                            let t = checkHref.getAttribute("data-href") || "";
                            "" != t && (window.location.href = t)
                        }
                    }
                    , O = t => {
                        k = t.changedTouches[t.changedTouches.length - 1],
                            I = k.pageX,
                            q = !0
                    }
                    , j = t => {
                        if (k = t.changedTouches[t.changedTouches.length - 1],
                            T = k.pageX,
                            q = !1,
                            checkHref = t.target,
                            I < T)
                            C(o || 1);
                        else if (I > T)
                            w(o || 1);
                        else {
                            let t = checkHref.getAttribute("data-href") || "";
                            "" != t && (window.location.href = t)
                        }
                    }
                    ;
                h.addEventListener("mousedown", W, {
                    passive: !0
                }),
                    h.addEventListener("touchstart", O, {
                        passive: !0
                    }),
                    h.addEventListener("mouseup", E, {
                        passive: !0
                    }),
                    h.addEventListener("touchend", j, {
                        passive: !0
                    })
            }
        }
        ))
    }
}

$("body").on("click", "#product_view_more", delay((function (t) {
    t.preventDefault();
    var e = $(this).closest(".detail__list");
    var i = $(this);
    page++;
    let a = {
        count: $(this).data("count"),
        page: page
    };
    page == 1 && e.find(".list-products").empty(),
        $.ajax({
            type: "POST",
            data: a,
            url: "/tim-kiem",
            beforeSend: function () {
                loadingBox("open")
            },
            success: function (t) {
                loadingBox("close"),
                    0 == t.status || (e.find(".list-products").append(t.html),
                        t.hasMore || i.addClass("hidden_more"))
            },
            error: function (t) {
                loadingBox("close")
            }
        })
}), 500));

$(document).ready(function () {
    $(".tab-button").click(function () {
        var t = $(this).data("tab");
        $(".banner").hide(),
            $(".tab-button").removeClass("active"),
            $("#" + t).show(),
            $(this).addClass("active");
        var e = $("header").outerHeight();
        $("html, body").animate({
            scrollTop: $("#" + t).offset().top - e
        }, 1000)
    });
    var t = $(".popup-backdrop");
    $(".promotion_detail").hide();
    $(".popup_mini").hide();
    $(".btn_view").click(function () {
        $(".promotion_detail").fadeToggle(),
            t.fadeToggle()
    });
    $(".rule_view").click(function () {
        $(".popup_mini").fadeToggle(),
            t.fadeToggle()
    });
    t.click(function () {
        $(".popup_mini").fadeOut(),
            $(".promotion_detail").fadeOut(),
            t.fadeOut()
    });

    var h = $(".detail-category").height();
    if (h < 300) {
        $(".detail-category .btn-main").hide();
    }

    $('.js-btn-close-search').on("click", function () {
        $('.header-overlay').removeClass('active')
        $('.suggest-search').removeClass('active')
    })

    $('#inp-search').on("click", function () {
        $('.header-overlay').addClass('active')
        $('.suggest-search').addClass('active')
    })

    $('.header-overlay').on("click", function () {
        $('.header-overlay').removeClass('active')
        $('.suggest-search').removeClass('active')
    })

});

var old_toch = {
    clientX: 106,
    screenX: 106
};

function unify(t) {
    if (["iPad Simulator", "iPhone Simulator", "iPod Simulator", "iPad", "iPhone", "iPod"].includes(navigator.platform) || navigator.userAgent.includes("Mac") && "ontouchend" in document) {
        const e = t.changedTouches ? t.changedTouches[0] : t.touches ? t.touches[0] : t;
        return void 0 !== e && (old_toch = e),
            old_toch
    }
    return t.changedTouches ? t.changedTouches[0] : t
}



;

//tinybox

(function (e, t, n) {
    var r = function (e) {
        this.options = e;
        this.init()
    };
    r.prototype = {
        constructor: r,
        init: function () {
            var r = this,
                i = r.options,
                s = r.$box = i.target.css("opacity", 0),
                o = s[0].id;
            e(t).on("keydown", function (e) {
                if (e.keyCode === 27) {
                    r.hide()
                }
            });
            e("[data-role=tinybox-closer][data-id=" + o + "]").on("click", function (e) {
                e.preventDefault();
                r.hide()
            });
            if (i.flexible) {
                e(n).resize(function () {
                    s.css("margin-left", -s.width() / 2)
                })
            };
            //e("[data-role=tinybox-trigger][href=#" + o + "]").on("click", function (e) {
            //    e.preventDefault();
            //    r.show()
            //})
        },
        show: function () {
            var n = this,
                r = n.options,
                i = n.$box,
                s = e(".tinybox:visible"),
                o = r.top,
                u = false;
            if (o === "auto") {
                o = -i.height() / 2
            } else {
                switch (typeof o) {
                    case "function":
                        o = o.call(this, i);
                        break;
                    default:
                        u = true;
                        i.css({
                            top: o
                        })
                }
            }
            i.css({
                marginLeft: -i.innerWidth() / 2,
                position: "fixed",
                zIndex: 888889,
                left: "50%",
                display: "block"
            });
            if (!u) {
                i.css({
                    marginTop: o,
                    top: "40%"
                })
            }
            s.stop().animate({
                opacity: 0
            }, r.speed, function () {
                this.style.display = "none"
            });
            i.stop().animate({
                opacity: 1
            }, r.speed);
            var a = e("#tinybox-overlay");
            if (!a[0]) {
                e(t.body).append('<div id="tinybox-overlay"></div>');
                a = e("#tinybox-overlay").css({
                    display: "none",
                    background: r.background,
                    zIndex: 888888,
                    position: "fixed",
                    left: 0,
                    top: 0,
                    width: "100%",
                    height: "100%",
                    opacity: 0
                })
            }
            a.on("click", function () {
                n.hide()
            });
            a.stop().css("display", "block").animate({
                opacity: r.opacity
            }, r.speed, function () {
                i.trigger("tinybox:oncomplete")
            });
            i.trigger("tinybox:onshow")
        },
        hide: function () {
            var t = this.options,
                n = this.$box,
                r = e("#tinybox-overlay");
            r.add(n).stop().animate({
                opacity: 0
            }, t.speed, function () {
                this.style.display = "none";
                n.trigger("tinybox:onhide")
            })
        }
    };
    e.fn.tinybox = function (t) {
        t = e.extend({
            target: e(this),
            background: "#000",
            opacity: .7,
            speed: 400,
            top: "auto",
            flexible: false
        }, t);
        return new r(t)
    }
})(jQuery, document, window);


$(document).ready(function () {

    $(".phone-number").each(function (index) {
        var phone = $(this).text();
        $(this).html(formatPhoneNumber(phone))
    });

    $(".js-format-price").each(function () {
        var price = $(this).attr("data-price");
        $(this).html(formatPrice(price))
    });

    $(".ex_pricesale ").each(function () {

        var price = $(this).attr("data-price");
        var price1 = $(this).attr("data-price1");

        if (price1 > price) {
            $(this).removeClass('d-none');
            $(this).html(100 - (Math.round((price / price1) * 100)) + '%')
        }

    });

});


;