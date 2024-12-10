// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname; /
    const menuLinks = document.querySelectorAll("#nav a"); 

    menuLinks.forEach(link => link.classList.remove("active"));

    menuLinks.forEach(link => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });

});


