// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname; // Đường dẫn hiện tại
    const menuLinks = document.querySelectorAll("#nav a"); // Menu chính
    const iconLinks = document.querySelectorAll(".header-icons a"); // Icon liên kết

    // Bỏ 'active' trước khi cập nhật
    menuLinks.forEach(link => link.classList.remove("active"));
    iconLinks.forEach(link => link.classList.remove("active"));

    // Thêm class 'active' chỉ cho menu chính khớp với đường dẫn
    menuLinks.forEach(link => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });

    // Tránh thêm 'active' cho icon hoặc liên kết với href="#"
    iconLinks.forEach(link => {
        if (link.getAttribute("href") !== "#" && link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });
});


