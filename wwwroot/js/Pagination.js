let currentPage = 1;
const rowsPerPage = 10;
const allRows = document.querySelectorAll('.row');
let filteredRows = Array.From(allRows);
let totalPages = Math.ceil(filteredRows.length / rowsPerPage);

function displayPage(page) {

    allRows.forEach(page => page.style.display = 'none');

    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;

    for (let i = start; i < end && i < filteredRows.length; i++)
    {
        filteredRows[i].style.display = 'block';
    }
    document.getElementById('page-info').textContent =
        `Page ${page} of ${totalPages || 1}`;


}

function nextPage() {
    if (currentPage < totalPages) {
        currentPages++;
        showPage(currentPage);

    }


}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        showPage(currentPage);
    }

}


