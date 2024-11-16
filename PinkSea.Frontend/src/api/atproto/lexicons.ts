import type { Oekaki } from '@/models/oekaki'

declare module '@atcute/client/lexicons' {
  type EmptyParams = object
  interface GenericTimelineQueryRequest {
    since?: Date | null,
    limit?: number | null,
  }
  interface GenericTimelineQueryOutput {
    oekaki: Oekaki[]
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetIdentity {
    interface Output {
      did: string,
      handle: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaPutOekaki {
    interface Input {
      data: string,
      nsfw: boolean,
      alt: string,
      tags: string[] | undefined,
      parent: string | undefined,
      bskyCrosspost: boolean | undefined
    }
    interface Output {
      uri: string,
      rkey: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetAuthorFeed {
    interface Params extends GenericTimelineQueryRequest {
      did: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetTagFeed {
    interface Params extends GenericTimelineQueryRequest {
      tag: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetOekaki {
    interface Params {
      did: string,
      rkey: string
    }

    interface Output {
      parent: Oekaki,
      children: Oekaki[]
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetHandleFromDid {
    interface Params {
      did: string
    }

    interface Output {
      handle: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaBeginLoginFlow {
    interface Input {
      handle: string,
      redirectUrl: string,
      password?: string | null
    }

    interface Output {
      redirect?: string | undefined,
      failureReason?: string | undefined
    }
  }

  interface Queries {
    'com.shinolabs.pinksea.getRecent': {
      params: GenericTimelineQueryRequest,
      output: GenericTimelineQueryOutput
    },
    'com.shinolabs.pinksea.getAuthorFeed': {
      params: ComShinolabsPinkseaGetAuthorFeed.Params,
      output: GenericTimelineQueryOutput,
    },
    'com.shinolabs.pinksea.getTagFeed': {
      params: ComShinolabsPinkseaGetTagFeed.Params,
      output: GenericTimelineQueryOutput,
    },
    'com.shinolabs.pinksea.getIdentity': {
      params: EmptyParams,
      output: ComShinolabsPinkseaGetIdentity.Output
    },
    'com.shinolabs.pinksea.getOekaki': {
      params: ComShinolabsPinkseaGetOekaki.Params,
      output: ComShinolabsPinkseaGetOekaki.Output
    },
    'com.shinolabs.pinksea.getHandleFromDid': {
      params: ComShinolabsPinkseaGetHandleFromDid.Params,
      output: ComShinolabsPinkseaGetHandleFromDid.Output
    }
  }

  interface Procedures {
    'com.shinolabs.pinksea.putOekaki': {
      input: ComShinolabsPinkseaPutOekaki.Input,
      output: ComShinolabsPinkseaPutOekaki.Output
    },
    'com.shinolabs.pinksea.refreshSession': {
      input: EmptyParams,
      output: EmptyParams
    },
    'com.shinolabs.pinksea.invalidateSession': {
      input: EmptyParams,
      output: EmptyParams
    },
    'com.shinolabs.pinksea.beginLoginFlow': {
      input: ComShinolabsPinkseaBeginLoginFlow.Input,
      output: ComShinolabsPinkseaBeginLoginFlow.Output
    }
  }
}

