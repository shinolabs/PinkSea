import type { Oekaki } from '@/models/oekaki'

declare module '@atcute/client/lexicons' {
  type EmptyParams = object
  interface GenericTimelineQueryOuput {
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
      tags: string[] | undefined
    }
    interface Output {
      uri: string,
      rkey: string
    }
  }

  interface Queries {
    'com.shinolabs.pinksea.getRecent': {
      params: EmptyParams,
      output: GenericTimelineQueryOuput
    },
    'com.shinolabs.pinksea.getIdentity': {
      params: EmptyParams,
      output: ComShinolabsPinkseaGetIdentity.Output
    }
  }

  interface Procedures {
    'com.shinolabs.pinksea.putOekaki': {
      input: ComShinolabsPinkseaPutOekaki.Input,
      output: ComShinolabsPinkseaPutOekaki.Output
    }
  }
}

