document.getElementById("btnLocation").addEventListener("click", async () => {
    const locationDisplay = document.getElementById("locationDiv");
    locationDisplay.classList.remove("hidden"); // Tailwind visibility

    const cancelBtn = document.getElementById("cancelBtn");
    cancelBtn.classList.remove("hidden"); // Tailwind visibility
});