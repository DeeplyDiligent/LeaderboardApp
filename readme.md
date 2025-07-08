# Leaderboard App - Setup Guide

## Useful Docker Commands

Below are some useful Docker commands for setting up and managing your environment:

```bash
# Create a Docker network
docker network create ghcac-bridge

# Run PostgreSQL container with custom configurations
docker run -d --name ghcac-db --network ghcac-bridge -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=challenge -e POSTGRES_DB=ghcac-db  -v postgres_data:/var/lib/postgresql/ghcac -p 5432:5432 postgres

# Inspect Docker network details
docker network inspect ghcac-bridge

# List all running Docker containers
docker ps

# List all Docker networks
docker network ls

# Connect a running container to the specified network
docker network connect ghcac-bridge LeaderboardApp

# Disconnect a running container from the specified network
docker network disconnect ghcac-bridge LeaderboardApp

# Stop a running container
docker stop LeaderboardApp
```

## Package Manager Console Commands
Use the following commands in the Package Manager Console for database scaffolding and migration:
```bash	
# Scaffold the DbContext for PostgreSQL
Scaffold-DbContext "Host=localhost;Database=ghcac-db;Username=postgres;Password=challenge" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models -Force

# Update the database after changes to the model
Update-Database

# Add a new migration
Add-Migration InitialMigration

# Remove the last migration
Remove-Migration
```

If error and need to fix connection string in the DbContext (inside OnConfiguring method), update the .cs file as below or remove the OnConfiguring method completely:
```csharp
{
    if (!optionsBuilder.IsConfigured)
    {
        // The connection string should come from the application's configuration
        // optionsBuilder.UseNpgsql("Host=localhost;Database=ghcac-db;Username=postgres;Password=challenge");
    }
}
```

## Database Create Table Scripts
The following SQL scripts create the necessary tables for the leaderboard application:

```sql
CREATE TABLE Teams (
    TeamId UUID PRIMARY KEY,
    Name VARCHAR NOT NULL,
    Icon VARCHAR NOT NULL,
    Tagline VARCHAR NOT NULL
);

CREATE TABLE Participants (
    ParticipantId UUID PRIMARY KEY, -- This was formerly `Members`
    FirstName VARCHAR NOT NULL,
    LastName VARCHAR NOT NULL,
    NickName VARCHAR NOT NULL,
    Email VARCHAR NOT NULL UNIQUE,
    TeamId UUID REFERENCES Teams(TeamId) ON DELETE CASCADE,
    ExternalId UUID NOT NULL,
    GitHubHandle VARCHAR,
    MsLearnHandle VARCHAR,
    Passcode VARCHAR,
    PasscodeExpiration TIMESTAMP WITH TIME ZONE,
    RefreshToken VARCHAR,
    LastLogin TIMESTAMP WITH TIME ZONE
);

CREATE TABLE LeaderboardEntries (
    LeaderboardEntryId UUID PRIMARY KEY,
    TeamId UUID REFERENCES Teams(TeamId) ON DELETE CASCADE,
    TeamName VARCHAR NOT NULL,
    Score INTEGER NOT NULL,
    LastUpdated TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE Activities (
    ActivityId SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    WeightType VARCHAR(50) NOT NULL CHECK (WeightType IN ('Fixed', 'Multiplier')),
    Weight DECIMAL(10, 2) NOT NULL,
    Scope VARCHAR(50) NOT NULL CHECK (Scope IN ('Individual', 'Team')),
    Frequency VARCHAR(50) NOT NULL CHECK (Frequency IN ('Once', 'Daily', 'Weekly'))
);

CREATE TABLE ParticipantScores (
    ScoreId SERIAL PRIMARY KEY,
    ParticipantId UUID NOT NULL,
    ActivityId INT NOT NULL,
    Score DECIMAL(10, 2) NOT NULL,
    Timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ParticipantId) REFERENCES Participants(ParticipantId) ON DELETE CASCADE, 
    FOREIGN KEY (ActivityId) REFERENCES Activities(ActivityId)
);

CREATE TABLE TeamDailySummaries (
    SummaryId SERIAL PRIMARY KEY,
    TeamId UUID REFERENCES Teams(TeamId) ON DELETE CASCADE,
    Day DATE NOT NULL,
    TotalSuggestionsCount INTEGER NOT NULL,
    TotalAcceptancesCount INTEGER NOT NULL,
    TotalLinesSuggested INTEGER NOT NULL,
    TotalLinesAccepted INTEGER NOT NULL,
    TotalActiveUsers INTEGER NOT NULL,
    TotalChatAcceptances INTEGER NOT NULL,
    TotalChatTurns INTEGER NOT NULL,
    TotalActiveChatUsers INTEGER NOT NULL
);

``` 

## Database Inserts
Use the following SQL scripts to insert test data into the database:


### Scoring System

```sql
SET IDENTITY_INSERT Activities ON;

INSERT INTO Activities (ActivityId, Name, WeightType, Weight, Scope, Frequency) VALUES
(1, 'ActiveUsersPerDay', 'Multiplier', 50.00, 'User', 'Daily'),
(2, 'EngagedUsersPerDay', 'Multiplier', 75.00, 'User', 'Daily'),
(3, 'TotalCodeSuggestions', 'Multiplier', 1.00, 'Team', 'Daily'),
(4, 'TotalLinesAccepted', 'Multiplier', 1.50, 'Team', 'Daily'),
(5, 'TotalChats', 'Multiplier', 10.00, 'Team', 'Daily'),
(6, 'TotalChatInsertions', 'Multiplier', 50.00, 'Team', 'Daily'),
(7, 'TotalChatCopyEvents', 'Multiplier', 30.00, 'Team', 'Daily'),
(8, 'TotalPRSummariesCreated', 'Multiplier', 100.00, 'Team', 'Daily'),
(9, 'TotalDotComChats', 'Multiplier', 20.00, 'Team', 'Daily'),
(10, 'CompletedLearningModule', 'Fixed', 250.00, 'User', 'Once'),
(11, 'GitHubCopilotCertificationExam', 'Fixed', 1000.00, 'User', 'Once'),
(12, 'CopilotDailyChallengeCompleted', 'Fixed', 200.00, 'User', 'Daily'),
(13, 'LinkClicked', 'Fixed', 50.00, 'User', 'Once'),
(14, 'TeamBonus', 'Fixed', 50.00, 'Team', 'Once');

SET IDENTITY_INSERT Activities OFF;

INSERT INTO Challenges (Title, Content, PostedDate, ActivityId)
VALUES 
(
    'Introduction to GitHub Copilot',
    'Complete the <strong>Introduction to GitHub Copilot</strong> module (19 min / 7 Units). GitHub Copilot uses OpenAI Codex to suggest code and entire functions in real time, right from your editor. <a href="https://learn.microsoft.com/en-us/training/modules/introduction-to-github-copilot/" target="_blank">Start Learning</a>',
    SYSDATETIME(),
    10
),
(
    'GitHub Copilot Certification Challenge',
    'Validate your Copilot skills and earn your badge by completing the <a href="https://learn.microsoft.com/en-us/credentials/certifications/github-copilot-technical-skills/" target="_blank">GitHub Copilot Technical Skills Certification</a>.',
    SYSDATETIME(),
    11
),
(
    'Try a link - its beautiful',
    '<a href="#" onclick="clickLink(17, ''https://code.visualstudio.com/blogs/2025/02/12/next-edit-suggestions''); return false;"> Next Edit Suggestions (NES) </a>...',
    SYSDATETIME(),
    13
),
(
    'Getting Code Suggestions in SQL with Copilot',
    'Explore GitHub Copilot’s real-time SQL code suggestions inside Azure Data Studio. Try writing joins or describing your goal in comments to see Copilot suggest full queries! <br><br>Example: <pre>SELECT [UserId], [Red], [Orange], [Yellow], [Green], [Blue], [Purple], [Rainbow] FROM [Tag].[Scoreboard] INNER JOIN</pre> Copilot may suggest a join automatically. <br><br>Or try natural language comments for code generation.',
    SYSDATETIME(),
    14
);

```

### Sample Data

```sql
INSERT INTO Teams (TeamId, Name, Icon, Tagline)
VALUES 
    ('f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', 'Team Alpha', 'alpha-icon.png', 'Leading the way'),
    ('c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', 'Team Bravo', 'bravo-icon.png', 'Bravery in action');

INSERT INTO Participants (ParticipantId, FirstName, LastName, NickName, Email, TeamId, ExternalId, GitHubHandle, MsLearnHandle)
VALUES 
    ('8d84c1e5-3c6a-4c61-82b8-74ad5e9c1d34', 'John', 'Doe', 'Johnny', 'john.doe@example.com', 'f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', '67f1b972-d5c4-4efc-82a3-9a4a4b7e2c32', 'johnny_github', 'john_mslearn'),
    ('6a8c1c32-4f2b-4f5d-91b7-81ad7e9c2d45', 'Jane', 'Smith', 'Janie', 'jane.smith@example.com', 'c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', 'e1b7c46e-2a5b-4e3f-8f2a-9a6c9b5a1e13', 'janie_github', 'jane_mslearn');

INSERT INTO LeaderboardEntries (LeaderboardEntryId, TeamId, TeamName, Score, LastUpdated)
VALUES 
    ('b74a2c74-9f7d-4a5a-bd92-8491a3c1b8b4', 'f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', 'Team Alpha', 250, '2024-08-12 10:00:00'),
    ('e92c3a6e-4a3b-4c4f-bf8b-72b3e9c1f5c6', 'c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', 'Team Bravo', 200, '2024-08-12 11:00:00');

INSERT INTO ParticipantScores (ParticipantId, ActivityId, Score)
VALUES 
    ('8d84c1e5-3c6a-4c61-82b8-74ad5e9c1d34', 1, 100), -- John Doe used GitHub Copilot
    ('6a8c1c32-4f2b-4f5d-91b7-81ad7e9c2d45', 2, 150); -- Jane Smith completed a learning module with a score multiplier

INSERT INTO TeamDailySummaries (TeamId, Day, TotalSuggestionsCount, TotalAcceptancesCount, TotalLinesSuggested, TotalLinesAccepted, TotalActiveUsers, TotalChatAcceptances, TotalChatTurns, TotalActiveChatUsers) 
VALUES
('f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', CURRENT_DATE, 1000, 800, 1800, 1200, 10, 32, 200, 4),
('c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', CURRENT_DATE, 800, 600, 1100, 700, 12, 57, 426, 8),
('f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', CURRENT_DATE - INTERVAL '1 day', 1500, 1100, 2200, 1600, 15, 40, 300, 6),
('c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', CURRENT_DATE - INTERVAL '1 day', 1200, 900, 1500, 1000, 14, 60, 500, 10),
('f7b6a37c-6147-4f3d-8c6c-7b0e9c3c16a1', CURRENT_DATE - INTERVAL '2 days', 1300, 1000, 1900, 1400, 13, 45, 350, 7),
('c4a4a5e7-2a5d-4f3f-8e6c-7d8e9c3c17a2', CURRENT_DATE - INTERVAL '2 days', 1100, 800, 1300, 900, 11, 50, 400, 9);

```

### GitHub Copilot Survey
```sql
INSERT INTO Surveys (SurveyId, Title, Description)
VALUES
    ('5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'GitHub Copilot Developer Survey', 'Survey to measure the impact of GitHub Copilot');


-- Short-format survey example questions
INSERT INTO SurveyQuestions (QuestionId, SurveyId, QuestionText, QuestionType)
VALUES
    ('1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'How would you feel if you could no longer use GitHub Copilot?', 'MultipleChoice'),
    ('2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'Please explain your answer:', 'Text'),
    ('3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'When using GitHub Copilot, I…', 'Rating'),
    ('4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'What challenges have you encountered in using GitHub Copilot since your last survey?', 'MultipleChoice'),
    ('ab8a849c-14cf-494f-b71a-6a84dc7cfb1c', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'In using GitHub Copilot in the last week…', 'Text');

-- Long-format survey example questions
INSERT INTO SurveyQuestions (QuestionId, SurveyId, QuestionText, QuestionType)
VALUES
    ('bce8b84a-1af2-4ae2-bcf5-7175331efa67', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'I use GitHub Copilot to…', 'MultipleChoice'),
    ('4a9a46c4-3a06-475d-8b08-0ecd3dbb5ee6', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'When using GitHub Copilot, my team…', 'Rating'),
    ('f9824a6d-61fe-434d-9f85-183db7604b29', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'Which of the following GitHub Copilot enablement materials have you received?', 'MultipleChoice'),
    ('fcd75fa8-7c3e-4069-8ad9-4047ac86cbc4', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'What challenges have you encountered in using GitHub Copilot since your last survey?', 'MultipleChoice'),
    ('8bdd80c2-02a9-422c-99d1-fb3cfc32bacb', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'What’s your job title?', 'MultipleChoice'),
    ('d72b5002-66da-4f24-93f8-0bf6d635efb4', '5f3e5e07-2f43-42e4-83c5-9b9d6c4b1234', 'May we contact you for follow-up? If so, leave your email address.', 'Text');
      
   
-- Options for "How would you feel if you could no longer use GitHub Copilot?"
INSERT INTO SurveyOptions (OptionId, QuestionId, OptionText)
VALUES
    ('aa1b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c5d', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'Very disappointed'),
    ('bb2c3d4e-5f6a-7b8c-9d0e-1f2a3b4c5d6e', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'Somewhat disappointed'),
    ('cc3d4e5f-6a7b-8c9d-0e1f-2a3b4c5d6e7f', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'Not disappointed'),
    ('778594d6-1dd0-4e3f-b81f-d832f5194123', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'N/A - I no longer use Copilot');

-- Options for "When using GitHub Copilot, I…"
INSERT INTO SurveyOptions (OptionId, QuestionId, OptionText)
VALUES
    ('a7087cb0-d7cf-439f-941d-e33ff20e2321', '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', 'Strongly Disagree'),
    ('6e8b8e21-adcc-4bce-9bd1-8b91fb93d360', '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', 'Disagree'),
    ('eec4ab14-5204-4236-8a60-6fecc5d6cb0a', '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', 'Neutral'),
    ('0f4de557-13d0-4073-bdb6-6db87475d54e', '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', 'Agree'),
    ('7715e343-03e7-42a5-9cc1-7ace5008e87d', '3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f', 'Strongly Agree');

-- Options for "What challenges have you encountered in using GitHub Copilot since your last survey?"
INSERT INTO SurveyOptions (OptionId, QuestionId, OptionText)
VALUES
    ('b338e3b3-fa08-474b-b378-f8025aaafad8', '4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', 'I never received a Copilot suggestion'),
    ('c78bbe13-8fc3-4889-97d3-af58f17ca268', '4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', 'Copilot was slow to respond'),
    ('1e51fed3-153f-4575-86f9-8b6886ba4c01', '4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', 'Copilot generated incorrect code'),
    ('960b9600-b601-4f16-bf2d-ca695d40c694', '4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', 'Copilot suggestions were not relevant'),
    ('e2b717d5-06c4-4fdf-8772-0d8d6e66f40c', '4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f99', 'N/A - I have not encountered any challenges');

```
