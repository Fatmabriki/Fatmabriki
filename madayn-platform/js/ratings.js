// Rating system binder for surveys, news, and services
(function(){
  const app = window.MADAYN || {};
  function bindWidget(root){
    const stars = Array.from(root.querySelectorAll('.stars i'));
    let current = 0;
    stars.forEach((star)=>{
      star.addEventListener('mouseenter', ()=> highlight(+star.dataset.rating));
      star.addEventListener('mouseleave', ()=> highlight(current));
      star.addEventListener('click', ()=>{
        current = +star.dataset.rating;
        highlight(current);
        promptAndSubmit(current);
      });
    });
    function highlight(n){
      stars.forEach((s,i)=> s.classList.toggle('active', i < n));
    }
    function promptAndSubmit(rating){
      const typeAttr = root.getAttribute('data-type') || 'survey';
      const id = +(root.getAttribute('data-id') || '0');
      Swal.fire({
        title: 'أضف تعليقاً (اختياري)', input: 'textarea', inputPlaceholder: 'شاركنا رأيك...', showCancelButton: true, confirmButtonText: 'تأكيد التقييم', cancelButtonText: 'إلغاء'
      }).then(res=>{
        if (res.isDismissed) return;
        const comment = res.value || '';
        if (typeAttr === 'survey') {
          app.api.rateSurvey(id, rating, comment);
        } else if (typeAttr === 'news') {
          app.api.rateNews(id, rating, comment);
        } else if (typeAttr === 'service-consultant') {
          app.api.rateService('Consultant', id, rating, comment);
        } else if (typeAttr === 'service-contractor') {
          app.api.rateService('Contractor', id, rating, comment);
        }
        Swal.fire({ icon:'success', title: 'تم تسجيل تقييمك' });
      });
    }
  }

  function bindAll(){
    document.querySelectorAll('.rating-widget').forEach(bindWidget);
  }
  window.MADAYN_RATINGS = { bindAll };
  document.addEventListener('DOMContentLoaded', bindAll);
})();

