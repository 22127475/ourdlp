# GitHub Workflows Documentation

This directory contains GitHub Actions workflows for the OURDLP project.

## Workflows Overview

### 1. CI/CD Pipeline (`ci-cd.yml`)
**Purpose:** Automated continuous integration and deployment

**Triggers:**
- Push to `main`, `develop`, or `feature/**` branches
- Pull requests to `main` or `develop` branches

**Actions:**
- Checkout code
- Setup .NET environment
- Restore dependencies
- Build the project in Release configuration
- Run tests

**Status:** ✅ Ready to use (will adapt when actual .NET project files are added)

---

### 2. Branch Protection (`branch-protection.yml`)
**Purpose:** Enforce code review requirements for main and develop branches

**Triggers:**
- Pull requests to `main` or `develop` branches

**Actions:**
- Checks if PR has at least one approval
- Fails if no approvals exist
- Provides guidance for setting up branch protection rules

**Additional Setup Required:**
To fully enforce this, configure branch protection in GitHub repository settings:
1. Go to **Settings** > **Branches** > **Add branch protection rule**
2. Add rules for `main` and `develop` branches
3. Enable: **Require a pull request before merging**
4. Enable: **Require approvals** (set minimum to 1)
5. Optional: **Require status checks to pass** (select CI/CD workflow)

---

### 3. Email Notifications (`email-notifications.yml`)
**Purpose:** Send scheduled email notifications to issue assignees

**Schedule:**
- Runs every **Monday and Friday at 9 AM UTC**
- Can be triggered manually via workflow dispatch

**Logic:**
- Fetches all open issues with assignees
- Identifies issues that need notification:
  - Regular reminder: Monday and Friday
  - Urgent reminder: Issues with deadlines < 3 days (checked daily by urgent-deadline-check.yml)
- Groups notifications by assignee
- Displays notification summary in workflow logs

**Deadline Detection:**
- Uses milestone due dates to determine issue deadlines
- Calculates days until deadline
- Flags urgent issues (< 3 days)

**Current Implementation:**
- Displays notifications in workflow logs
- Does not actually send emails yet

**To Enable Email Sending:**
1. Choose an email service provider (SMTP-based)
2. Add repository secrets:
   - `MAIL_SERVER` - SMTP server address
   - `MAIL_PORT` - SMTP port (typically 587 or 465)
   - `MAIL_USERNAME` - SMTP username
   - `MAIL_PASSWORD` - SMTP password/app password
   - `MAIL_FROM` - Sender email address
3. Add an email action step using `dawidd6/action-send-mail` or similar
4. Update workflow to format and send emails

**Example email action step:**
```yaml
- name: Send email
  uses: dawidd6/action-send-mail@v3
  with:
    server_address: ${{ secrets.MAIL_SERVER }}
    server_port: ${{ secrets.MAIL_PORT }}
    username: ${{ secrets.MAIL_USERNAME }}
    password: ${{ secrets.MAIL_PASSWORD }}
    subject: "Issue Assignment Reminder"
    to: ${{ assignee.email }}
    from: ${{ secrets.MAIL_FROM }}
    body: |
      You have open issues assigned to you...
```

---

### 4. Daily Urgent Deadline Check (`urgent-deadline-check.yml`)
**Purpose:** Daily check for issues with approaching deadlines

**Schedule:**
- Runs **daily at 9 AM UTC**
- Can be triggered manually via workflow dispatch

**Logic:**
- Checks all open issues
- Identifies issues with deadlines < 3 days
- Sends urgent notifications for these issues
- Runs independently from the Monday/Friday schedule

**Current Implementation:**
- Displays urgent notifications in workflow logs
- Follows same email setup requirements as email-notifications.yml

---

## Setup Instructions

### Prerequisites
1. Repository with GitHub Actions enabled
2. Issues with assignees for notification testing
3. (Optional) Milestones with due dates for deadline tracking

### Basic Setup
1. Workflows are automatically active once merged to the main branch
2. No additional configuration needed for CI/CD and branch protection
3. Review workflow runs in the **Actions** tab

### Email Setup (Optional)
To enable actual email sending:
1. Choose an SMTP provider (Gmail, SendGrid, AWS SES, etc.)
2. Create account and get SMTP credentials
3. Add secrets to repository:
   - Go to **Settings** > **Secrets and variables** > **Actions**
   - Add new repository secrets
4. Modify `email-notifications.yml` and `urgent-deadline-check.yml` to include email sending steps

### Testing Workflows
- Use **workflow_dispatch** trigger to manually run workflows
- Go to **Actions** tab > Select workflow > **Run workflow**
- Check workflow logs to see notifications that would be sent

---

## Workflow Behavior Summary

| Day | Email Notifications | Urgent Deadline Check |
|-----|-------------------|----------------------|
| Monday | ✅ All open issues | ✅ Issues with deadline < 3 days |
| Tuesday | ❌ | ✅ Issues with deadline < 3 days |
| Wednesday | ❌ | ✅ Issues with deadline < 3 days |
| Thursday | ❌ | ✅ Issues with deadline < 3 days |
| Friday | ✅ All open issues | ✅ Issues with deadline < 3 days |
| Saturday | ❌ | ✅ Issues with deadline < 3 days |
| Sunday | ❌ | ✅ Issues with deadline < 3 days |

**Note:** Issues with deadline < 3 days receive notifications **every day** regardless of the Monday/Friday schedule.

---

## Maintenance

### Updating Schedules
To change notification times or days, modify the cron expressions:
```yaml
schedule:
  - cron: '0 9 * * 1,5'  # Monday and Friday at 9 AM UTC
  # Format: minute hour day-of-month month day-of-week
```

### Adding More Notification Conditions
Edit the JavaScript logic in the workflow files to add custom notification rules based on:
- Labels
- Priority
- Age of issue
- Custom fields in issue body

---

## Troubleshooting

### Workflow not running
- Check if workflow file syntax is valid (use GitHub's workflow editor)
- Verify triggers are correctly configured
- Check Actions tab for error messages

### Email notifications not working
- Verify repository secrets are correctly set
- Check SMTP credentials are valid
- Review workflow logs for error messages
- Test with a manual workflow run first

### Branch protection not enforcing
- Verify branch protection rules are configured in repository settings
- Check that workflow is included in required status checks
- Ensure workflow runs on PR events

---

## Security Notes

- Never commit SMTP passwords or credentials directly to workflow files
- Always use GitHub repository secrets for sensitive data
- Restrict access to repository secrets to necessary personnel
- Consider using app-specific passwords for email services
- Review workflow logs to ensure no sensitive data is exposed
