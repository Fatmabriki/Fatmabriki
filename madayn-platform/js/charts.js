// Charts and statistics for admin dashboard
(function(){
  const app = window.MADAYN;

  function loadAnalytics(){
    const ctx1 = document.getElementById('chart-completion');
    const ctx2 = document.getElementById('chart-ratings');
    const ctx3 = document.getElementById('chart-engagement');
    if (!ctx1 || !ctx2 || !ctx3) return;

    const surveys = app.api.listSurveys();
    const responses = app.utils.load('madayn_responses', []);
    const ratings = app.utils.load('madayn_survey_ratings', []);

    const completionCounts = surveys.map(s=> responses.filter(r=>r.surveyId==s.surveyId).length);
    new Chart(ctx1, { type: 'bar', data: { labels: surveys.map(s=> s.title), datasets: [{ label: 'إكمال الاستطلاع', data: completionCounts, backgroundColor: '#4A1A4A' }] }, options: { responsive: true } });

    const ratingAvg = surveys.map(s=>{
      const rs = ratings.filter(r=> r.surveyId==s.surveyId).map(r=>r.rating);
      return rs.length ? (rs.reduce((a,b)=>a+b,0)/rs.length).toFixed(2) : 0;
    });
    new Chart(ctx2, { type: 'line', data: { labels: surveys.map(s=> s.title), datasets: [{ label: 'متوسط التقييم', data: ratingAvg, borderColor: '#E89B5A', backgroundColor: 'rgba(232,155,90,.2)' }] }, options:{ responsive:true, tension:.3 } });

    const monthly = Array.from({ length: 12 }, (_,i)=> i+1);
    const engagement = monthly.map(m=> responses.filter(r=> new Date(r.completedAt).getMonth()+1===m).length);
    new Chart(ctx3, { type: 'bar', data: { labels: monthly.map(m=> `شهر ${m}`), datasets: [{ label: 'المشاركات', data: engagement, backgroundColor: '#B85450' }] }, options:{ responsive:true } });
  }

  document.addEventListener('DOMContentLoaded', loadAnalytics);
  window.loadAnalytics = loadAnalytics;
})();

