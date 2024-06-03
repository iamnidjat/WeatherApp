// Attach click event handler to all "X" buttons with the class "removeCityButton"
document.querySelectorAll('.removeCityButton').forEach(function (button) {
    button.addEventListener('click', function () {
        // Get the city name from the data attribute
        let cityName = this.getAttribute('data-city');
        // Remove the city and its corresponding button from the list visually
        let cityItem = this.parentNode;
        cityItem.parentNode.removeChild(cityItem);
        // Send an AJAX request to your server to remove the city from the list
        fetch('/Forecast/RemoveCity?city=' + cityName, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                // Check if the request was successful
                if (response.ok) {
                    // Optionally handle the response if needed
                    console.log('City removed successfully');
                } else {
                    console.error('Failed to remove city');
                }
            })
            .catch(error => {
                console.error('An error occurred while removing city:', error);
            });
    });
});
