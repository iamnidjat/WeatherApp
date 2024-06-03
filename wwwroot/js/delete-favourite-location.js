$('.remove-btn').click(function () {
    let locationId = $(this).data('id');

    fetch('/FavouriteLocations/DeleteFLocation?fLocationId=' + locationId, {
        method: 'DELETE',
    }).then(response => {
        if (response.ok) {
            $(this).closest('li').remove();
        } else {
            console.error('Failed to remove location');
        }
    }).catch(error => {
            console.error('An error occurred while removing the location:', error);
        });
});