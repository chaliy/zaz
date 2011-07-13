/*jslint onevar: true, undef: true, newcap: true, regexp: true, plusplus: true, bitwise: true, devel: true, maxerr: 50 */
/*global window: true, jQuery:true, $:true, document:true*/
/// <reference path="vendor/jquery/1.5.1/jquery.js"/>
$(function () {

    // Error
    $(document).ajaxError(function (ev, xhr, settings, errorThrown) {
        alert(xhr.responseText);
    });

    $.ajax({
        url: "../MetaList?v=3",
        context: document.body,
        dataType: "json",
        success: function (data, status, xhr) {
            var content = $("#commandListTemplate").tmpl({ commands: data });
            $("#main").replaceWith(content);
        }
    });
}); 