document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname;
    const allLinks = document.querySelectorAll("a"); 

    // Loại bỏ tất cả class 'active'
    allLinks.forEach(link => link.classList.remove("active"));

    // Thêm class 'active' cho liên kết phù hợp
    allLinks.forEach(link => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });
});
