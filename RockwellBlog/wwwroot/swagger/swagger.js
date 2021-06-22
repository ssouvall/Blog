setTimeout(SetHref, 1000);

//Write a function that allows us to link back to our blog from the custom image
function SetHref() {
    let anchor = document.querySelector(".topbar-wrapper a");
    anchor.href = "/Home/Index";
}