// Surveys: dynamic form, progress, validation, autosave, submission, rating
(function(){
  const app = window.MADAYN;
  const autosaveKey = (id) => `madayn_autosave_survey_${id}`;

  function createSurveyForm(surveyData){
    const container = document.getElementById('survey-fields');
    const title = document.getElementById('survey-title');
    const progressEl = document.getElementById('survey-progress');
    const btnPrev = document.getElementById('btn-prev');
    const btnNext = document.getElementById('btn-next');
    const btnSubmit = document.getElementById('btn-submit');
    const btnSave = document.getElementById('btn-save');
    if (!container) return;

    title.textContent = surveyData.title;
    const questions = app.api.listQuestions(surveyData.surveyId);
    const total = questions.length;

    const saved = app.utils.load(autosaveKey(surveyData.surveyId), {});
    const answers = { ...saved };
    let index = 0;

    function render(){
      const q = questions[index];
      container.innerHTML = '';
      const field = document.createElement('div');
      field.className = 'vstack gap-2';
      field.innerHTML = `
        <label class="form-label ${q.isRequired? 'required':''}">${q.questionText}</label>
        <div id="field-holder"></div>
        <div class="form-text">${index+1} من ${total}</div>`;
      container.appendChild(field);
      const holder = document.getElementById('field-holder');
      let input;
      if (q.questionType === 'Text') {
        input = document.createElement('textarea'); input.className = 'form-control'; input.rows = 4; input.value = answers[q.questionId] ?? '';
      } else if (q.questionType === 'Rating') {
        input = document.createElement('div'); input.className = 'rating-widget'; input.innerHTML = `
          <div class="stars">
            <i class="fas fa-star" data-rating="1"></i>
            <i class="fas fa-star" data-rating="2"></i>
            <i class="fas fa-star" data-rating="3"></i>
            <i class="fas fa-star" data-rating="4"></i>
            <i class="fas fa-star" data-rating="5"></i>
          </div>`;
        setTimeout(()=>{ // bind stars
          const stars = Array.from(input.querySelectorAll('.stars i'));
          const setVal = (n)=>{ answers[q.questionId] = n; stars.forEach((s,i)=> s.classList.toggle('active', i < n)); save(); updateProgress(); };
          stars.forEach(st=>{
            st.addEventListener('mouseenter', ()=>{ const n=+st.dataset.rating; stars.forEach((s,i)=> s.classList.toggle('hover', i < n)); });
            st.addEventListener('mouseleave', ()=> stars.forEach(s=> s.classList.remove('hover')));
            st.addEventListener('click', ()=> setVal(+st.dataset.rating));
          });
          const preset = answers[q.questionId]; if (preset) setVal(preset);
        });
      } else if (q.questionType === 'YesNo') {
        input = document.createElement('div'); input.className = 'hstack gap-3'; input.innerHTML = `
          <div class="form-check">
            <input class="form-check-input" type="radio" name="yn" id="yn1" value="Yes">
            <label class="form-check-label" for="yn1">نعم</label>
          </div>
          <div class="form-check">
            <input class="form-check-input" type="radio" name="yn" id="yn2" value="No">
            <label class="form-check-label" for="yn2">لا</label>
          </div>`;
        setTimeout(()=>{
          const radios = input.querySelectorAll('input[type="radio"]');
          radios.forEach(r=> r.addEventListener('change', ()=>{ answers[q.questionId] = r.value; save(); updateProgress(); }));
          if (answers[q.questionId]) input.querySelector(`input[value="${answers[q.questionId]}"]`)?.setAttribute('checked','checked');
        });
      } else if (q.questionType === 'MultipleChoice') {
        input = document.createElement('div');
        const opts = JSON.parse(q.options || '[]');
        input.innerHTML = opts.map((opt,i)=>`<div class="form-check"><input class="form-check-input" type="checkbox" id="opt${i}"><label class="form-check-label" for="opt${i}">${opt}</label></div>`).join('');
        setTimeout(()=>{
          const checks = input.querySelectorAll('input[type="checkbox"]');
          const setVal = ()=>{ const val = opts.filter((_,i)=> checks[i].checked); answers[q.questionId] = val; save(); updateProgress(); };
          checks.forEach(c=> c.addEventListener('change', setVal));
          const preset = answers[q.questionId] || [];
          preset.forEach(v=>{ const idx = opts.indexOf(v); if (idx>-1) input.querySelector(`#opt${idx}`).checked = true; });
        });
      } else {
        input = document.createElement('input'); input.className='form-control'; input.type='text'; input.value = answers[q.questionId] ?? '';
      }
      holder.appendChild(input);
      updateProgress();
      btnPrev.disabled = index === 0;
      btnNext.classList.toggle('d-none', index === total-1);
      btnSubmit.classList.toggle('d-none', index < total-1);
    }

    function isAnswered(q){
      const v = answers[q.questionId];
      if (q.questionType === 'MultipleChoice') return Array.isArray(v) && v.length > 0;
      return v !== undefined && v !== '' && v !== null;
    }
    function validate(){
      const q = questions[index];
      if (q.isRequired && !isAnswered(q)) { Swal.fire({ icon:'warning', title:'أكمل هذا السؤال قبل المتابعة' }); return false; }
      return true;
    }
    function updateProgress(){
      const answered = questions.filter(q=> isAnswered(q)).length;
      const pct = Math.round((answered/total)*100);
      progressEl.style.width = pct + '%';
      progressEl.setAttribute('aria-valuenow', String(pct));
      progressEl.textContent = pct + '%';
    }
    function save(){ app.utils.save(autosaveKey(surveyData.surveyId), answers); }

    btnNext.addEventListener('click', ()=>{ if (!validate()) return; index = Math.min(questions.length-1, index+1); render(); });
    btnPrev.addEventListener('click', ()=>{ index = Math.max(0, index-1); render(); });
    btnSave.addEventListener('click', ()=>{ save(); Swal.fire({ icon:'success', title:'تم الحفظ' }); });
    btnSubmit.addEventListener('click', ()=>{
      for (const q of questions) { if (q.isRequired && !isAnswered(q)) { Swal.fire({ icon:'warning', title:`أكمل السؤال: ${q.questionText}` }); return; } }
      app.api.submitResponse(surveyData.surveyId, answers);
      localStorage.removeItem(autosaveKey(surveyData.surveyId));
      Swal.fire({ icon:'success', title:'شكراً لمشاركتك' }).then(()=>{
        const ratingHolder = document.getElementById('survey-rating');
        if (ratingHolder) {
          ratingHolder.innerHTML = `<div class="rating-widget" data-type="survey" data-id="${surveyData.surveyId}"><div class="stars"><i class="fas fa-star" data-rating="1"></i><i class="fas fa-star" data-rating="2"></i><i class="fas fa-star" data-rating="3"></i><i class="fas fa-star" data-rating="4"></i><i class="fas fa-star" data-rating="5"></i></div><span class="rating-text small">قيّم الاستطلاع</span></div>`;
          if (window.MADAYN_RATINGS) window.MADAYN_RATINGS.bindAll();
        }
      });
    });
    render();
  }

  function initTakeSurvey(){
    const id = +(app.utils.getQuery('id') || '0');
    if (!id) return;
    const survey = app.api.listSurveys().find(s=>s.surveyId==id);
    if (!survey) return;
    createSurveyForm(survey);
  }

  document.addEventListener('DOMContentLoaded', initTakeSurvey);
  window.createSurveyForm = createSurveyForm;
})();

