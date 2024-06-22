document.addEventListener('DOMContentLoaded', (event) => {
    const inputs = document.querySelectorAll('#noInsertInput');

    inputs.forEach(input => {
        // Disable paste event
        input.addEventListener('paste', (e) => {
            e.preventDefault();
            alert('Paste is disabled for this input field.');
        });

        // Disable copy event
        input.addEventListener('copy', (e) => {
            e.preventDefault();
            alert('Copy is disabled for this input field.');
        });

        // Disable cut event
        input.addEventListener('cut', (e) => {
            e.preventDefault();
            alert('Cut is disabled for this input field.');
        });

        // Disable context menu (right-click)
        input.addEventListener('contextmenu', (e) => {
            e.preventDefault();
            alert('Context menu is disabled for this input field.');
        });

        // Disable keyboard shortcuts
        input.addEventListener('keydown', (e) => {
            if ((e.ctrlKey && e.key === 'v') || (e.metaKey && e.key === 'v')) {
                e.preventDefault();
                alert('Paste is disabled for this input field.');
            }
            if ((e.ctrlKey && e.key === 'c') || (e.metaKey && e.key === 'c')) {
                e.preventDefault();
                alert('Copy is disabled for this input field.');
            }
            if ((e.ctrlKey && e.key === 'x') || (e.metaKey && e.key === 'x')) {
                e.preventDefault();
                alert('Cut is disabled for this input field.');
            }
        });
    });
});

