#!/usr/bin/env bash
# デモ用リポジトリ同梱: 現在のブランチから main へのプルリクエストを作る。
# origin の URL で GitBucket / GitHub を自動判定する。説明文は docs/pr-draft.md（1行目=タイトル、以降=本文）。
# 使い方: リポジトリのルートで  scripts/create-pr.sh [base]      （base 省略時 main）
set -euo pipefail
export GIT_TERMINAL_PROMPT=0

base="${1:-main}"
draft="docs/pr-draft.md"
[ -f "$draft" ] || { echo "説明文 $draft がありません。先に下書きを作ってください" >&2; exit 1; }

head="$(git rev-parse --abbrev-ref HEAD)"
[ "$head" != "$base" ] || { echo "現在のブランチが $base です。作業ブランチに切り替えてください" >&2; exit 1; }
origin="$(git remote get-url origin)"

title="$(sed -n '1p' "$draft" | sed -E 's/^#+[[:space:]]*//')"
body="$(sed '1d' "$draft")"

# 未 push なら push する（承認ダイアログが出るのは想定どおり）
if ! git ls-remote --exit-code --heads origin "$head" >/dev/null 2>&1; then
  git push -u origin "$head" >&2
fi

case "$origin" in
  *github.com*)
    command -v gh >/dev/null || { echo "GitHub CLI (gh) が必要です" >&2; exit 1; }
    gh pr create --base "$base" --head "$head" --title "$title" --body "$body"
    ;;
  *)
    # GitBucket: http(s)://host[:port]/git/<owner>/<repo>.git → API http(s)://host[:port]/api/v3
    api_base="${origin%/git/*}/api/v3"
    path="${origin#*/git/}"; path="${path%.git}"
    owner="${path%%/*}"; repo="${path#*/}"
    # 資格情報は git の credential helper から取る（up.sh が保存済み）
    creds="$(printf 'url=%s\n\n' "$origin" | git credential fill)"
    user="$(printf '%s\n' "$creds" | sed -n 's/^username=//p')"
    pass="$(printf '%s\n' "$creds" | sed -n 's/^password=//p')"
    [ -n "$user" ] || { echo "origin の資格情報が見つかりません（git credential）" >&2; exit 1; }
    payload="$(printf '%s' "$title" | awk 'BEGIN{ORS=""} {gsub(/\\/,"\\\\"); gsub(/"/,"\\\""); print}')"
    bodyj="$(printf '%s' "$body" | awk 'BEGIN{ORS=""} {gsub(/\\/,"\\\\"); gsub(/"/,"\\\""); gsub(/\t/,"\\t"); printf "%s\\n", $0}')"
    resp="$(curl -sS -u "$user:$pass" -H 'Content-Type: application/json' \
      -X POST "$api_base/repos/$owner/$repo/pulls" \
      -d "{\"title\":\"$payload\",\"head\":\"$head\",\"base\":\"$base\",\"body\":\"$bodyj\"}")"
    # 4.47 系は応答 JSON に生の改行が混ざるため、URL だけ抜く
    url="$(printf '%s' "$resp" | tr -d '\n\r' | sed -n 's/.*"html_url":"\([^"]*\)".*/\1/p')"
    [ -n "$url" ] || { echo "PR 作成に失敗しました: $resp" >&2; exit 1; }
    echo "$url"
    ;;
esac
