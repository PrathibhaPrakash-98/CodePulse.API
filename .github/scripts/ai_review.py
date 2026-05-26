import os, requests

GITHUB_TOKEN = os.environ["GITHUB_TOKEN"]
GEMINI_API_KEY = os.environ["GEMINI_API_KEY"]
REPO = os.environ["REPO"]
PR_NUMBER = os.environ["PR_NUMBER"]

# 1. Fetch PR diff
headers = {"Authorization": f"Bearer {GITHUB_TOKEN}", "Accept": "application/vnd.github.v3.diff"}
diff = requests.get(
    f"https://api.github.com/repos/{REPO}/pulls/{PR_NUMBER}",
    headers=headers
).text

# 2. Load coding standards
with open("CodePulse.API/docs/coding-standards.md") as f:
    standards = f.read()

# 3. Call Gemini (FREE)
prompt = f"""You are a .NET code reviewer. Review the PR diff below strictly against 
our coding standards. Flag violations with file name, line number, and explanation.

## Coding Standards:
{standards}

## PR Diff:
{diff}

Provide a structured review."""

response = requests.post(
    f"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={GEMINI_API_KEY}",
    json={"contents": [{"parts": [{"text": prompt}]}]}
)

review_body = response.json()["candidates"][0]["content"]["parts"][0]["text"]

# 4. Post comment on PR
requests.post(
    f"https://api.github.com/repos/{REPO}/pulls/{PR_NUMBER}/reviews",
    headers={"Authorization": f"Bearer {GITHUB_TOKEN}"},
    json={"body": review_body, "event": "COMMENT"}
)