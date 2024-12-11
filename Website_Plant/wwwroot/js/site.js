const links = document.querySelectorAll('a');

links.forEach(link => {
    link.addEventListener('click', (event) => {
        // Xóa class 'active' khỏi tất cả các link
        links.forEach(l => l.classList.remove('active'));
        // Gán class 'active' vào link được click
        link.classList.add('active');
    });
});