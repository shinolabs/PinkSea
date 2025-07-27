import type { Router } from 'vue-router'
import { xrpc } from './client';

export const withHandleDidRouteResolver = (router: Router): void => {
  router.beforeEach(async (to, from) => {
    if (to.params.did === undefined) {
      return
    }

    const routeDid = to.params.did as string
    if (routeDid.startsWith("did:")) {
      return
    }

    try {
      const { data } = await xrpc.get("com.atproto.identity.resolveHandle", { params: { handle: routeDid } })

      to.params.did = data.did
    } catch {
      return
    }
  });
};
