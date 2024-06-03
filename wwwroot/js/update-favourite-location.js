document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".edit-btn").forEach(button => {
        button.addEventListener("click", function () {
            const id = this.getAttribute("data-id");
            const nameSpan = document.querySelector(`.location-name[data-id="${id}"]`);
            const input = document.querySelector(`.edit-name-input[data-id="${id}"]`);
            const saveButton = document.querySelector(`.save-btn[data-id="${id}"]`);

            nameSpan.style.display = "none";
            input.style.display = "inline";
            input.value = nameSpan.textContent;
            this.style.display = "none";
            saveButton.style.display = "inline";
        });
    });

    document.querySelectorAll(".save-btn").forEach(button => {
        button.addEventListener("click", function () {
            const id = this.getAttribute("data-id");
            const input = document.querySelector(`.edit-name-input[data-id="${id}"]`);
            const newName = input.value;

            // Assuming you have an endpoint to handle the update request
            fetch(`/FavouriteLocations/UpdateFLocation?fLocationId=${id}`, {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ Name: newName })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const nameSpan = document.querySelector(`.location-name[data-id="${id}"]`);
                        nameSpan.textContent = newName;
                        nameSpan.style.display = "inline";
                        input.style.display = "none";
                        this.style.display = "none";
                        const editButton = document.querySelector(`.edit-btn[data-id="${id}"]`);
                        editButton.style.display = "inline";
                    } else {
                        alert("Failed to update location name.");
                    }
                })
                .catch(error => {
                    console.error("Error updating location name:", error);
                    alert("Error updating location name.");
                });
        });
    });
});
