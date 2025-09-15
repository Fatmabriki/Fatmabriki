/* Global App Bootstrap for Madayn Investment Survey Platform */
(function() {
  const app = window.MADAYN = window.MADAYN || {};

  // API endpoints (for backend integration later)
  app.apiEndpoints = {
    surveys: '/api/surveys',
    news: '/api/news',
    consultants: '/api/consultants',
    contractors: '/api/contractors',
    ratings: '/api/ratings',
    reports: '/api/reports'
  };

  // Utilities
  app.utils = {
    uuid: () => 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8); return v.toString(16);
    }),
    getQuery: (key) => new URLSearchParams(window.location.search).get(key),
    save: (key, val) => localStorage.setItem(key, JSON.stringify(val)),
    load: (key, fallback) => {
      try { const v = JSON.parse(localStorage.getItem(key)); return v ?? fallback; } catch { return fallback; }
    },
    nowIso: () => new Date().toISOString(),
    clamp: (n, min, max) => Math.max(min, Math.min(max, n))
  };

  // Storage keys
  const KEYS = {
    users: 'madayn_users',
    currentUser: 'madayn_current_user',
    surveys: 'madayn_surveys',
    questions: 'madayn_questions',
    responses: 'madayn_responses',
    surveyRatings: 'madayn_survey_ratings',
    news: 'madayn_news',
    newsRatings: 'madayn_news_ratings',
    comments: 'madayn_comments',
    consultants: 'madayn_consultants',
    contractors: 'madayn_contractors',
    serviceRatings: 'madayn_service_ratings',
    guestSession: 'madayn_guest_session'
  };

  // Seed sample data (Arabic realistic content)
  function seed() {
    if (!app.utils.load(KEYS.users)) {
      const admin = { userId: 1, name: 'مسؤول النظام', email: 'admin@madayn.om', password: 'Admin@123', role: 'Admin', isActive: true, createdAt: app.utils.nowIso() };
      const investor = { userId: 2, name: 'مستثمر تجريبي', email: 'user@madayn.om', password: 'User@123', role: 'Investor', isActive: true, createdAt: app.utils.nowIso() };
      app.utils.save(KEYS.users, [admin, investor]);
    }

    if (!app.utils.load(KEYS.surveys)) {
      const surveys = [
        { surveyId: 1, title: 'تقييم بيئة الاستثمار في مدائن', description: 'نهدف إلى تحسين الخدمات والبنية الأساسية في المدن الصناعية.', createdBy: 1, isActive: true, startDate: app.utils.nowIso(), endDate: null, createdAt: app.utils.nowIso() },
        { surveyId: 2, title: 'قياس رضا المستثمرين عن الخدمات الإلكترونية', description: 'شاركنا رأيك في الخدمات الرقمية المقدمة.', createdBy: 1, isActive: true, startDate: app.utils.nowIso(), endDate: null, createdAt: app.utils.nowIso() }
      ];
      app.utils.save(KEYS.surveys, surveys);
      const questions = [
        { questionId: 1, surveyId: 1, questionText: 'كيف تقيم جودة البنية التحتية؟', questionType: 'Rating', options: null, isRequired: 1, orderIndex: 1 },
        { questionId: 2, surveyId: 1, questionText: 'ما هي التحديات التي تواجهها؟', questionType: 'Text', options: null, isRequired: 0, orderIndex: 2 },
        { questionId: 3, surveyId: 1, questionText: 'هل تنصح المستثمرين الآخرين بالاستثمار في مدائن؟', questionType: 'YesNo', options: null, isRequired: 1, orderIndex: 3 },
        { questionId: 4, surveyId: 1, questionText: 'الخدمات الأكثر أهمية بالنسبة لك', questionType: 'MultipleChoice', options: JSON.stringify(['الخدمات اللوجستية','التراخيص','الدعم التمويلي','التسويق']), isRequired: 1, orderIndex: 4 },
        { questionId: 5, surveyId: 2, questionText: 'سهولة استخدام البوابة الإلكترونية', questionType: 'Rating', options: null, isRequired: 1, orderIndex: 1 },
        { questionId: 6, surveyId: 2, questionText: 'اقترحات للتحسين', questionType: 'Text', options: null, isRequired: 0, orderIndex: 2 }
      ];
      app.utils.save(KEYS.questions, questions);
    }

    if (!app.utils.load(KEYS.news)) {
      const news = [
        { newsId: 1, title: 'إطلاق خدمة تراخيص جديدة', content: 'أعلنت مدائن عن إطلاق نظام تراخيص إلكتروني متكامل لتسهيل الإجراءات على المستثمرين.', imageUrl: '', createdBy: 1, isPublished: 1, publishedAt: app.utils.nowIso(), createdAt: app.utils.nowIso() },
        { newsId: 2, title: 'افتتاح مصنع جديد في صحار', content: 'تم افتتاح مصنع جديد للصناعات التحويلية باستثمارات كبيرة وتوفير فرص عمل.', imageUrl: '', createdBy: 1, isPublished: 1, publishedAt: app.utils.nowIso(), createdAt: app.utils.nowIso() },
        { newsId: 3, title: 'برنامج تدريب للمؤسسات الصغيرة', content: 'إطلاق برنامج تدريب شامل لرفع كفاءة المؤسسات الصغيرة والمتوسطة.', imageUrl: '', createdBy: 1, isPublished: 1, publishedAt: app.utils.nowIso(), createdAt: app.utils.nowIso() }
      ];
      app.utils.save(KEYS.news, news);
    }

    if (!app.utils.load(KEYS.consultants)) {
      const consultants = [
        { consultantId: 1, name: 'بيت الخبرة للاستشارات', specialization: 'دراسات جدوى', description: 'فريق متخصص في إعداد دراسات الجدوى وخطط الأعمال.', contact: 'consult@example.com', imageUrl: '', isActive: 1, createdAt: app.utils.nowIso() },
        { consultantId: 2, name: 'الرؤية المستقبلية', specialization: 'تحول رقمي', description: 'استشارات التحول الرقمي وتبني التقنيات الحديثة.', contact: 'future@example.com', imageUrl: '', isActive: 1, createdAt: app.utils.nowIso() }
      ];
      app.utils.save(KEYS.consultants, consultants);
    }

    if (!app.utils.load(KEYS.contractors)) {
      const contractors = [
        { contractorId: 1, companyName: 'الأفق للمقاولات', specialization: 'إنشاءات صناعية', description: 'تنفيذ مشاريع مصانع ومستودعات بمعايير عالية.', contact: 'afuq@example.com', imageUrl: '', isActive: 1, createdAt: app.utils.nowIso() },
        { contractorId: 2, companyName: 'البنيان المتين', specialization: 'أعمال ميكانيكية وكهربائية', description: 'خبرة واسعة في أعمال MEP للمشاريع الكبرى.', contact: 'bonyan@example.com', imageUrl: '', isActive: 1, createdAt: app.utils.nowIso() }
      ];
      app.utils.save(KEYS.contractors, contractors);
    }

    if (!app.utils.load(KEYS.responses)) app.utils.save(KEYS.responses, []);
    if (!app.utils.load(KEYS.surveyRatings)) app.utils.save(KEYS.surveyRatings, []);
    if (!app.utils.load(KEYS.newsRatings)) app.utils.save(KEYS.newsRatings, []);
    if (!app.utils.load(KEYS.serviceRatings)) app.utils.save(KEYS.serviceRatings, []);
    if (!app.utils.load(KEYS.comments)) app.utils.save(KEYS.comments, []);
  }

  // Auth
  app.auth = {
    current: () => app.utils.load(KEYS.currentUser, null),
    isAdmin: () => (app.utils.load(KEYS.currentUser, null)?.role === 'Admin'),
    login: (email, password) => {
      const users = app.utils.load(KEYS.users, []);
      const found = users.find(u => u.email === email && u.password === password && u.isActive);
      if (found) { app.utils.save(KEYS.currentUser, found); return { ok: true, user: found }; }
      return { ok: false, message: 'بيانات الدخول غير صحيحة' };
    },
    logout: () => localStorage.removeItem(KEYS.currentUser),
    register: (name, email, password) => {
      const users = app.utils.load(KEYS.users, []);
      if (users.some(u => u.email === email)) return { ok: false, message: 'البريد مستخدم مسبقاً' };
      const user = { userId: users.length ? Math.max(...users.map(u => u.userId)) + 1 : 1, name, email, password, role: 'Investor', isActive: true, createdAt: app.utils.nowIso() };
      users.push(user); app.utils.save(KEYS.users, users); app.utils.save(KEYS.currentUser, user);
      return { ok: true, user };
    }
  };

  // Simple mock API facade (replace with real AJAX later)
  app.api = {
    listSurveys: () => app.utils.load(KEYS.surveys, []),
    listQuestions: (surveyId) => app.utils.load(KEYS.questions, []).filter(q => q.surveyId == surveyId).sort((a,b)=>a.orderIndex-b.orderIndex),
    saveSurvey: (survey) => {
      const surveys = app.utils.load(KEYS.surveys, []);
      if (!survey.surveyId) { survey.surveyId = surveys.length ? Math.max(...surveys.map(s => s.surveyId)) + 1 : 1; survey.createdAt = app.utils.nowIso(); surveys.push(survey); }
      else { const i = surveys.findIndex(s=>s.surveyId==survey.surveyId); if (i>-1) surveys[i] = { ...surveys[i], ...survey }; else surveys.push(survey); }
      app.utils.save(KEYS.surveys, surveys); return survey;
    },
    deleteSurvey: (surveyId) => { const surveys = app.utils.load(KEYS.surveys, []).filter(s=>s.surveyId!=surveyId); app.utils.save(KEYS.surveys, surveys); },
    submitResponse: (surveyId, payload) => {
      const responses = app.utils.load(KEYS.responses, []);
      const current = app.auth.current();
      const guest = ensureGuestSession();
      responses.push({ responseId: responses.length ? Math.max(...responses.map(r=>r.responseId)) + 1 : 1, surveyId, userId: current?.userId ?? null, guestSessionId: current ? null : guest, completedAt: app.utils.nowIso(), answers: payload });
      app.utils.save(KEYS.responses, responses); return { ok: true };
    },
    rateSurvey: (surveyId, rating, comment) => {
      const ratings = app.utils.load(KEYS.surveyRatings, []);
      const current = app.auth.current();
      ratings.push({ ratingId: ratings.length ? Math.max(...ratings.map(r=>r.ratingId)) + 1 : 1, surveyId, userId: current?.userId ?? null, rating, comment, createdAt: app.utils.nowIso() });
      app.utils.save(KEYS.surveyRatings, ratings); return { ok: true };
    },
    listNews: () => app.utils.load(KEYS.news, []).filter(n => n.isPublished),
    saveNews: (news) => { const items = app.utils.load(KEYS.news, []); if (!news.newsId) { news.newsId = items.length ? Math.max(...items.map(n=>n.newsId)) + 1 : 1; news.createdAt = app.utils.nowIso(); } const i = items.findIndex(n=>n.newsId==news.newsId); if (i>-1) items[i]=news; else items.push(news); app.utils.save(KEYS.news, items); return news; },
    rateNews: (newsId, rating, comment) => { const ratings = app.utils.load(KEYS.newsRatings, []); const current = app.auth.current(); ratings.push({ ratingId: ratings.length ? Math.max(...ratings.map(r=>r.ratingId)) + 1 : 1, newsId, userId: current?.userId ?? null, rating, comment, createdAt: app.utils.nowIso() }); app.utils.save(KEYS.newsRatings, ratings); return { ok: true }; },
    listConsultants: () => app.utils.load(KEYS.consultants, []).filter(c=>c.isActive),
    listContractors: () => app.utils.load(KEYS.contractors, []).filter(c=>c.isActive),
    saveConsultant: (c) => { const arr = app.utils.load(KEYS.consultants, []); if (!c.consultantId) { c.consultantId = arr.length ? Math.max(...arr.map(x=>x.consultantId)) + 1 : 1; c.createdAt = app.utils.nowIso(); c.isActive = 1; arr.push(c); } else { const i=arr.findIndex(x=>x.consultantId==c.consultantId); if (i>-1) arr[i] = { ...arr[i], ...c }; } app.utils.save(KEYS.consultants, arr); return c; },
    saveContractor: (c) => { const arr = app.utils.load(KEYS.contractors, []); if (!c.contractorId) { c.contractorId = arr.length ? Math.max(...arr.map(x=>x.contractorId)) + 1 : 1; c.createdAt = app.utils.nowIso(); c.isActive = 1; arr.push(c); } else { const i=arr.findIndex(x=>x.contractorId==c.contractorId); if (i>-1) arr[i] = { ...arr[i], ...c }; } app.utils.save(KEYS.contractors, arr); return c; },
    rateService: (serviceType, serviceId, rating, review) => { const arr = app.utils.load(KEYS.serviceRatings, []); const current = app.auth.current(); arr.push({ ratingId: arr.length ? Math.max(...arr.map(r=>r.ratingId)) + 1 : 1, serviceType, serviceId, userId: current?.userId ?? null, rating, review, createdAt: app.utils.nowIso() }); app.utils.save(KEYS.serviceRatings, arr); return { ok: true }; },
    commentsFor: (type, id) => app.utils.load(KEYS.comments, []).filter(c=>c.type===type && c.itemId==id),
    addComment: (type, id, text) => { const arr = app.utils.load(KEYS.comments, []); const current = app.auth.current(); if (!current) return { ok:false, message:'يلزم تسجيل الدخول للتعليق' }; arr.push({ commentId: arr.length? Math.max(...arr.map(c=>c.commentId))+1:1, type, itemId: id, userId: current.userId, text, createdAt: app.utils.nowIso() }); app.utils.save(KEYS.comments, arr); return { ok: true }; }
  };

  function ensureGuestSession() {
    let id = app.utils.load(KEYS.guestSession, null);
    if (!id) { id = app.utils.uuid(); app.utils.save(KEYS.guestSession, id); }
    return id;
  }

  // Component loader
  function loadIncludes() {
    const includeNodes = document.querySelectorAll('[data-include]');
    includeNodes.forEach(async (node) => {
      const path = node.getAttribute('data-include');
      try { const res = await fetch(path); const html = await res.text(); node.innerHTML = html; if (path.includes('navigation.html')) initNavbar(); }
      catch (e) { node.innerHTML = '<div class="text-danger">تعذر تحميل المكون</div>'; }
    });
  }

  function initNavbar() {
    const current = app.auth.current();
    const loginBtn = document.querySelector('[data-action="login"]');
    const logoutBtn = document.querySelector('[data-action="logout"]');
    const userSpan = document.querySelector('[data-user-name]');
    if (current) {
      if (userSpan) userSpan.textContent = current.name;
      if (loginBtn) loginBtn.classList.add('d-none');
      if (logoutBtn) logoutBtn.classList.remove('d-none');
      if (app.auth.isAdmin()) {
        document.querySelectorAll('[data-admin-only]').forEach(el=>el.classList.remove('d-none'));
      }
    } else {
      if (loginBtn) loginBtn.classList.remove('d-none');
      if (logoutBtn) logoutBtn.classList.add('d-none');
      document.querySelectorAll('[data-admin-only]').forEach(el=>el.classList.add('d-none'));
    }
    const logoutLink = document.querySelector('[data-action="logout"]');
    if (logoutLink) logoutLink.addEventListener('click', (e)=>{ e.preventDefault(); app.auth.logout(); Swal.fire({ icon:'success', title:'تم تسجيل الخروج' }).then(()=> location.reload()); });
  }

  // Public module helpers
  app.public = {
    renderSurveyList: () => {
      const container = document.getElementById('surveys-list'); if (!container) return;
      const list = app.api.listSurveys().filter(s=>s.isActive);
      container.innerHTML = list.map(s => `
        <div class="col-md-6 col-lg-4">
          <div class="card hover-raise h-100">
            <div class="card-body">
              <h5 class="card-title text-primary-600">${s.title}</h5>
              <p class="card-text text-muted">${s.description ?? ''}</p>
              <a class="btn btn-primary" href="/workspace/madayn-platform/pages/public/take-survey.html?id=${s.surveyId}">المشاركة الآن</a>
            </div>
          </div>
        </div>`).join('');
    },
    renderNewsFeed: () => {
      const container = document.getElementById('news-feed'); if (!container) return;
      const items = app.api.listNews();
      let page = 0; const pageSize = 5;
      const renderPage = () => {
        const slice = items.slice(page*pageSize, (page+1)*pageSize);
        if (!slice.length) return;
        container.insertAdjacentHTML('beforeend', slice.map(n => `
          <article class="card hover-raise mb-3">
            <div class="card-body">
              <h5 class="card-title">${n.title}</h5>
              <p class="card-text">${n.content}</p>
              <div class="rating-widget" data-type="news" data-id="${n.newsId}">
                <div class="stars">
                  <i class="fas fa-star" data-rating="1"></i>
                  <i class="fas fa-star" data-rating="2"></i>
                  <i class="fas fa-star" data-rating="3"></i>
                  <i class="fas fa-star" data-rating="4"></i>
                  <i class="fas fa-star" data-rating="5"></i>
                </div>
                <span class="rating-text small">قيم الخبر</span>
              </div>
              <div class="mt-2">
                <button class="btn btn-sm btn-accent" data-action="comment" data-id="${n.newsId}"><i class="fa-regular fa-comment"></i> تعليق</button>
              </div>
              <div class="comments mt-2" id="comments-news-${n.newsId}"></div>
            </div>
          </article>`).join(''));
        if (window.MADAYN_RATINGS) window.MADAYN_RATINGS.bindAll();
        bindComments(slice.map(s=>s.newsId));
        page++;
      };
      renderPage();
      window.addEventListener('scroll', () => {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 200) renderPage();
      });
      function bindComments(ids) {
        ids.forEach(id=>{
          const btn = container.querySelector(`[data-action="comment"][data-id="${id}"]`);
          if (!btn) return;
          btn.addEventListener('click', () => {
            Swal.fire({
              title: 'أضف تعليقاً', input: 'textarea', inputLabel: 'التعليق', inputPlaceholder: 'اكتب تعليقك هنا...', showCancelButton: true, confirmButtonText: 'نشر', cancelButtonText: 'إلغاء'
            }).then(res=>{
              if (res.isConfirmed && res.value?.trim()) {
                const r = app.api.addComment('news', id, res.value.trim());
                if (!r.ok) return Swal.fire({ icon:'warning', title:r.message });
                renderComments(id);
              }
            });
          });
          renderComments(id);
        });
      }
      function renderComments(id) {
        const holder = document.getElementById(`comments-news-${id}`);
        const list = app.api.commentsFor('news', id);
        holder.innerHTML = list.map(c=>{
          const user = app.utils.load(KEYS.users, []).find(u=>u.userId==c.userId);
          return `<div class="comment"><div class="author">${user?.name ?? 'مستخدم'}</div><div class="text">${c.text}</div></div>`;
        }).join('');
      }
    },
    renderDirectory: (type) => {
      const container = document.getElementById('directory-list'); if (!container) return;
      const data = type === 'consultants' ? app.api.listConsultants() : app.api.listContractors();
      const termInput = document.getElementById('search-term');
      const specInput = document.getElementById('search-spec');
      const render = () => {
        const term = termInput?.value?.trim() ?? '';
        const spec = specInput?.value ?? '';
        const filtered = data.filter(item => {
          const t = term === '' || (item.name||item.companyName).includes(term) || (item.description||'').includes(term);
          const s = !spec || (item.specialization||'') === spec;
          return t && s;
        });
        container.innerHTML = filtered.map(item=>{
          const id = item.consultantId ?? item.contractorId;
          const title = item.name ?? item.companyName;
          const spec = item.specialization ?? '';
          return `<div class="col-md-6 col-xl-4"><div class="card hover-raise h-100"><div class="card-body">
            <h5 class="card-title text-primary-600">${title}</h5>
            <div class="text-muted mb-2">${spec}</div>
            <p class="card-text">${item.description ?? ''}</p>
            <div class="rating-widget" data-type="${type==='consultants'?'service-consultant':'service-contractor'}" data-id="${id}">
              <div class="stars">
                <i class="fas fa-star" data-rating="1"></i>
                <i class="fas fa-star" data-rating="2"></i>
                <i class="fas fa-star" data-rating="3"></i>
                <i class="fas fa-star" data-rating="4"></i>
                <i class="fas fa-star" data-rating="5"></i>
              </div>
              <span class="rating-text small">قيم الخدمة</span>
            </div>
            <a class="btn btn-sm btn-accent mt-2" href="mailto:${item.contact}"><i class="fa-regular fa-envelope"></i> تواصل</a>
          </div></div></div>`;
        }).join('');
        if (window.MADAYN_RATINGS) window.MADAYN_RATINGS.bindAll();
      };
      render();
      termInput?.addEventListener('input', render);
      specInput?.addEventListener('change', render);
    }
  };

  // Admin helpers (minimal; enhanced in each page)
  app.admin = {
    bindDataTable: (selector) => { if (!window.jQuery || !window.jQuery.fn?.DataTable) return; window.jQuery(selector).DataTable({ language: { url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/ar.json' } }); },
    renderUsers: () => {
      const table = document.getElementById('users-table'); if (!table) return;
      const users = app.utils.load(KEYS.users, []);
      const tbody = table.querySelector('tbody');
      tbody.innerHTML = users.map(u=>`<tr><td>${u.userId}</td><td>${u.name}</td><td>${u.email}</td><td>${u.role}</td><td>${u.isActive? 'نشط':'موقوف'}</td></tr>`).join('');
      app.admin.bindDataTable('#users-table');
    }
  };

  // Startup
  document.addEventListener('DOMContentLoaded', () => {
    seed();
    loadIncludes();
    // Page-level initializers
    app.public.renderSurveyList();
    app.public.renderNewsFeed();
    const dirType = document.body.getAttribute('data-directory');
    if (dirType) app.public.renderDirectory(dirType);
  });
})();

