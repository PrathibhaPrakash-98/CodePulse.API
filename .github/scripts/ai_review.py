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
   f"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={GEMINI_API_KEY}",
    json={"contents": [{"parts": [{"text": prompt}]}]}
)

response_json = response.json()
print("Gemini API Response:", response_json)  # ← this will show the actual error

# Check for errors
if "candidates" not in response_json:
    error_msg = response_json.get("error", {}).get("message", "Unknown Gemini API error")
    raise Exception(f"Gemini API Error: {error_msg}")

review_body = response_json["candidates"][0]["content"]["parts"][0]["text"]

# 4. Post comment on PR
requests.post(
    f"https://api.github.com/repos/{REPO}/pulls/{PR_NUMBER}/reviews",
    headers={"Authorization": f"Bearer {GITHUB_TOKEN}"},
    json={"body": review_body, "event": "COMMENT"}
)