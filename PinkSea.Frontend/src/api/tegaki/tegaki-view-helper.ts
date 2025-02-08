import { Tegaki } from '@/api/tegaki/tegaki';
import { useTegakiViewStore } from '@/api/tegaki/store'
import type { Router } from 'vue-router'
import i18next from 'i18next'

window.addEventListener('popstate', () => {
  const store = useTegakiViewStore();
  store.pop = true;
});

export const withTegakiViewBackProtection = (router: Router) : void => {
  router.beforeEach((to, from) => {
    console.log(`visible ${Tegaki.visible}`)
    if (!Tegaki.visible) {
      return true;
    }

    const confirm = window.confirm(i18next.t("tegakijs.confirmCancel"));
    if (confirm) {
      Tegaki.hide();
      return true;
    }

    return false;
  });
};
