// Lấy tất cả các liên kết trong menu
const menuLinks = document.querySelectorAll('.menu-list .menu-item a');

// Lặp qua từng liên kết
menuLinks.forEach(link => {
    link.addEventListener('click', function () {
        // Xóa class menu-item--active khỏi tất cả các mục menu
        document.querySelectorAll('.menu-item').forEach(item => {
            item.classList.remove('menu-item--active');
        });

        // Thêm class menu-item--active vào mục menu được nhấn
        this.parentElement.classList.add('menu-item--active');
    });
});

// Ghi nhớ trạng thái khi tải lại trang
window.addEventListener('DOMContentLoaded', () => {
    const currentUrl = window.location.pathname;
    menuLinks.forEach(link => {
        if (link.getAttribute('href') === currentUrl) {
            link.parentElement.classList.add('menu-item--active');
        }
    });
});
