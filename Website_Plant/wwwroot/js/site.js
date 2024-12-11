document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname; // Đường dẫn hiện tại
    const allLinks = document.querySelectorAll("a"); // Chọn tất cả liên kết

    // Loại bỏ tất cả class 'active'
    allLinks.forEach(link => link.classList.remove("active"));

    // Thêm class 'active' cho liên kết phù hợp
    allLinks.forEach(link => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active");
        }
    });
});
