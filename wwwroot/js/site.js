// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    const links = document.querySelectorAll('.nav-link');

links.forEach(link => {
        link.addEventListener('click', function () {
            links.forEach(l => l.classList.remove('active')); // remove active sa iba
            this.classList.add('active'); // add active sa pinindot
        });
});

