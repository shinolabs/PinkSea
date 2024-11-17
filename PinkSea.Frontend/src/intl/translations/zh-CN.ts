export default {
  sidebar: {
    title: 'PinkSea',
    tag: '绘图留言版',
    shinolabs: '一个 shinonome laboratories 项目',
  },
  menu: {
    greeting: '您好 @{{name}}！',
    invitation: '登录以开始创作！',
    input_placeholder: '@alice.bsky.social',
    password: '密码（可选）',
    atp_login: '@ 登录',
    my_oekaki: '我的绘图',
    recent: '最近',
    settings: '设置',
    logout: '登出',
    create_something: '创建绘图',
    oauth2_info:
      '如果您在密码框处留空，PinkSea 将使用 OAuth2 登录您的 PDS。这比密码登录更安全。',
  },
  breadcrumb: {
    recent: '最近',
    painter: '画师',
    settings: '您的设置',
    user_profile: '{{handle}} 的个人资料',
    user_post: '{{handle}} 的帖文',
    tagged: '打上 #{{tag}} 标签的帖文',
  },
  timeline: {
    by_before_handle: '作者 ',
    by_after_handle: '',
  },
  post: {
    response_from_before_handle: '',
    response_from_after_handle: ' 的回应，在 ',
    response_from_at_date: '', // in Chinese we don't use any words to seperate date and time
  },
  response_box: {
    login_to_respond: '登录以回应！',
    click_to_respond: '点击打开绘画面板',
    open_painter: '打开画板',
    reply: '回复！',
    cancel: '取消',
  },
  settings: {
    category_general: '通用',
    general_language: '语言',

    category_sensitive: '敏感媒体',
    sensitive_blur_nsfw: '模糊 NSFW 帖文',
    sensitive_hide_nsfw: '不显示 NSFW 帖文',
  },
  painter: {
    do_you_want_to_restore: '上一个上传报错了，但您的图像已保存。要恢复吗？',
    could_not_send_post:
      '上传帖文时出现问题。请稍后再试。您的帖文已保存在您的浏览器中。',
    add_a_description: '添加描述！',
    tag: '标签',
    crosspost_to_bluesky: '交叉发布到 Bluesky',
    upload: '上传！',
  },
  profile: {
    bluesky_profile: 'Bluesky 个人资料',
    domain: '网站',
  },
}
