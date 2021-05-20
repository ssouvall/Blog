/*!
* Start Bootstrap - Clean Blog v5.1.0 (https://startbootstrap.com/theme/clean-blog)
* Copyright 2013-2021 Start Bootstrap
* Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-clean-blog/blob/master/LICENSE)
*/
(function ($) {
    "use strict"; // Start of use strict

    // Floating label headings for the contact form
    $("body").on("input propertychange", ".floating-label-form-group", function (e) {
        $(this).toggleClass("floating-label-form-group-with-value", !!$(e.target).val());
    }).on("focus", ".floating-label-form-group", function () {
        $(this).addClass("floating-label-form-group-with-focus");
    }).on("blur", ".floating-label-form-group", function () {
        $(this).removeClass("floating-label-form-group-with-focus");
    });

    // Show the navbar when the page is scrolled up
    var MQL = 992;

})(jQuery); // End of use strict

//change header image based on screen size

const masthead = document.getElementById("masthead");

if (window.innerWidth < 800) {
    masthead.classList.add("small-banner");
} else {
    masthead.classList.remove("small-banner");
};


//make carousel dynamic by adding "active" to first slide
$(document).ready(function () {
    $('.carousel-item').first().addClass('active');
    $('.carouselExampleControls > li').first().addClass('active');
});

//Display file name in input when uploaded
function processSelectedFiles(fileInput) {
    var files = fileInput.files;
    var label = document.querySelectorAll(".control-label");

    //for (var i = 0; i < files.length; i++) {
    //    for (var j = 0; j < label.length; j++) {
    //        label[j].innerText = files[i].name;
    //    }
        
    //}
    for (var i = 0; i < label.length; i++) {
        for (var j = 0; j < files.length; j++) {
            label[i].innerHTML = files[j].name;
        }
    }
}