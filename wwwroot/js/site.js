// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.querySelectorAll('.toggle-password').forEach(btn => {
    btn.addEventListener('click', () => {
        const input = btn.parentElement.querySelector('input');
        const icon = btn.querySelector('i');
        const show = input.type === 'password';
        input.type = show ? 'text' : 'password';
        icon.className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
    });
});

function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = document.getElementById('deleteSpan_' + uniqueId);
    var confirmSpan = document.getElementById('confirmDeleteSpan_' + uniqueId);
    deleteSpan.style.display = isDeleteClicked ? 'none' : '';
    confirmSpan.style.display = isDeleteClicked ? '' : 'none';
}
document.addEventListener("DOMContentLoaded", function () {
    var sidebar = document.getElementById("sidebar");
    var toggleButton = document.getElementById("toggleMenu");
    if (!sidebar || !toggleButton) return;

    // Retrouver l'état du menu sur les autres pages
    if (localStorage.getItem("sidebarOpen") === "1") {
        sidebar.classList.remove("d-none");
    }

    toggleButton.addEventListener("click", function () {
        sidebar.classList.toggle("d-none");
        localStorage.setItem("sidebarOpen", sidebar.classList.contains("d-none") ? "0" : "1");
    });
});