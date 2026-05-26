import os, requests

GITHUB_TOKEN = os.environ["GITHUB_TOKEN"]
GROQ_API_KEY = os.environ["GROQ_API_KEY"]
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

# 3. Call Groq (FREE)
prompt = f"""You are a .NET code reviewer. Review the PR diff below strictly against 
our coding standards. Flag violations with file name, line number, and explanation.

## Coding Standards:
{standards}

## PR Diff:
{diff}

Provide a structured review."""

response = requests.post(
    "https://api.groq.com/openai/v1/chat/completions",
    headers={
        "Authorization": f"Bearer {GROQ_API_KEY}",
        "Content-Type": "application/json"
    },
    json={
        "model": "llama-3.3-70b-versatile",  # free model
        "messages": [{"role": "user", "content": prompt}],
        "max_tokens": 2000
    }
)

response_json = response.json()
print("Groq Response:", response_json)

if "choices" not in response_json:
    raise Exception(f"Groq API Error: {response_json}")

review_body = response_json["choices"][0]["message"]["content"]

# 4. Post comment on PR
requests.post(
    f"https://api.github.com/repos/{REPO}/pulls/{PR_NUMBER}/reviews",
    headers={"Authorization": f"Bearer {GITHUB_TOKEN}"},
    json={"body": review_body, "event": "COMMENT"}
)