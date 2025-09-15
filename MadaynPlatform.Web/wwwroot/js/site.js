// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
    window.MadaynApp = {
        Rating: {
            init: function () {
                document.querySelectorAll('.rating-widget .submit-rating')?.forEach(function (btn) {
                    btn.addEventListener('click', async function () {
                        const root = btn.closest('.rating-widget');
                        const entityType = root.dataset.entityType;
                        const entityId = root.dataset.entityId;
                        const selected = root.querySelector('.stars input[name="rating"]:checked');
                        const rating = selected ? parseInt(selected.value) : 5;
                        const comment = root.querySelector('textarea')?.value || '';
                        const resp = await fetch(`/api/api/surveys/${entityId}/rate`, {
                            method: 'POST', headers: { 'Content-Type': 'application/json' },
                            body: JSON.stringify({ ratingValue: rating, comment })
                        });
                        if (resp.ok) {
                            const data = await resp.json();
                            alert('تم الإرسال. المتوسط الجديد: ' + data.average.toFixed(2));
                        }
                    });
                });
            }
        }
    };
    document.addEventListener('DOMContentLoaded', function(){
        window.MadaynApp.Rating.init();
    });
})();
