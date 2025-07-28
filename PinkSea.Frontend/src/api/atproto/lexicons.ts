import type { Oekaki } from '@/models/oekaki'
import type { SearchType } from '@/models/search-type'
import type { Author } from '@/models/author'
import type { TagSearchResult } from '@/models/tag-search-result'
import type Profile from '@/models/profile'

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
  namespace ComShinolabsPinkseaPutProfile {
    interface Input {
      profile: {
        nickname?: string | null,
        bio?: string | null,
        avatar?: {
          uri: string,
          cid: string
        },
        links?: {
          link: string,
          name: string
        }[]
      }
    }
    interface Output {
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaDeleteOekaki {
    interface Input {
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

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetParentForReply {
    interface Params {
      did: string,
      rkey: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaUnspeccedGetProfile {
    interface Params {
      did: string
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace ComShinolabsPinkseaGetSearchResults {
    interface Params {
      query: string,
      type: SearchType,
      since?: Date | null,
      limit?: number | null
    }

    interface Output {
      oekaki?: Oekaki[] | null,
      tags?: TagSearchResult[] | null,
      profiles?: Author[] | null
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
    'com.shinolabs.pinksea.getAuthorReplies': {
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
    },
    'com.shinolabs.pinksea.getParentForReply': {
      params: ComShinolabsPinkseaGetParentForReply.Params,
      output: ComShinolabsPinkseaGetParentForReply.Params
    },
    'com.shinolabs.pinksea.getProfile': {
      params: ComShinolabsPinkseaUnspeccedGetProfile.Params,
      output: Profile
    },
    'com.shinolabs.pinksea.getSearchResults': {
      params: ComShinolabsPinkseaGetSearchResults.Params,
      output: ComShinolabsPinkseaGetSearchResults.Output
    }
  }

  interface Procedures {
    'com.shinolabs.pinksea.putOekaki': {
      input: ComShinolabsPinkseaPutOekaki.Input,
      output: ComShinolabsPinkseaPutOekaki.Output
    },
    'com.shinolabs.pinksea.putProfile': {
      input: ComShinolabsPinkseaPutProfile.Input,
      output: ComShinolabsPinkseaPutProfile.Output
    },
    'com.shinolabs.pinksea.deleteOekaki': {
      input: ComShinolabsPinkseaDeleteOekaki.Input,
      output: EmptyParams
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

