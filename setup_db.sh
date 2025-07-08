#!/bin/bash
# setup_db.sh
# Usage: ./setup_db.sh

# Get connection string from environment variable
CONN_STR="$psqlConnString"

if [ -z "$CONN_STR" ]; then
  echo "Error: Environment variable 'psqlConnString' is not set."
  exit 1
fi

SQL_FILE=$(mktemp)

cat > "$SQL_FILE" <<'EOF'
CREATE TABLE IF NOT EXISTS Teams (
    TeamId UUID PRIMARY KEY,
    Name VARCHAR NOT NULL,
    Icon VARCHAR NOT NULL,
    Tagline VARCHAR NOT NULL
);

CREATE TABLE IF NOT EXISTS Participants (
    ParticipantId UUID PRIMARY KEY,
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

CREATE TABLE IF NOT EXISTS LeaderboardEntries (
    LeaderboardEntryId UUID PRIMARY KEY,
    TeamId UUID REFERENCES Teams(TeamId) ON DELETE CASCADE,
    TeamName VARCHAR NOT NULL,
    Score INTEGER NOT NULL,
    LastUpdated TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE IF NOT EXISTS Activities (
    ActivityId SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    WeightType VARCHAR(50) NOT NULL CHECK (WeightType IN ('Fixed', 'Multiplier')),
    Weight DECIMAL(10, 2) NOT NULL,
    Scope VARCHAR(50) NOT NULL CHECK (Scope IN ('Individual', 'Team')),
    Frequency VARCHAR(50) NOT NULL CHECK (Frequency IN ('Once', 'Daily', 'Weekly'))
);

CREATE TABLE IF NOT EXISTS ParticipantScores (
    ScoreId SERIAL PRIMARY KEY,
    ParticipantId UUID NOT NULL,
    ActivityId INT NOT NULL,
    Score DECIMAL(10, 2) NOT NULL,
    Timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ParticipantId) REFERENCES Participants(ParticipantId) ON DELETE CASCADE, 
    FOREIGN KEY (ActivityId) REFERENCES Activities(ActivityId)
);

CREATE TABLE IF NOT EXISTS TeamDailySummaries (
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

CREATE TABLE IF NOT EXISTS Surveys (
    SurveyId UUID PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS SurveyQuestions (
    QuestionId UUID PRIMARY KEY,
    SurveyId UUID REFERENCES Surveys(SurveyId) ON DELETE CASCADE,
    QuestionText TEXT NOT NULL,
    QuestionType VARCHAR(50) CHECK (QuestionType IN ('Text', 'MultipleChoice', 'Rating', 'Boolean')) NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS SurveyOptions (
    OptionId UUID PRIMARY KEY,
    QuestionId UUID REFERENCES SurveyQuestions(QuestionId) ON DELETE CASCADE,
    OptionText VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS SurveyResponses (
    ResponseId UUID PRIMARY KEY,
    SurveyId UUID REFERENCES Surveys(SurveyId) ON DELETE CASCADE,
    ParticipantId UUID REFERENCES Participants(ParticipantId) ON DELETE CASCADE,
    SubmittedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS ResponseAnswers (
    AnswerId UUID PRIMARY KEY,
    ResponseId UUID REFERENCES SurveyResponses(ResponseId) ON DELETE CASCADE,
    QuestionId UUID REFERENCES SurveyQuestions(QuestionId) ON DELETE CASCADE,
    AnswerText TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO Activities (ActivityId, Name, WeightType, Weight, Scope, Frequency) VALUES
(1, 'UsedGithubCopilot', 'Fixed', 500.00, 'Individual', 'Daily'),
(2, 'CompletedLearningModule', 'Multiplier', 1.00, 'Individual', 'Once'),
(3, 'TotalSuggestionsCount', 'Multiplier', 3.00, 'Team', 'Daily'),
(4, 'TotalAcceptancesCount', 'Multiplier', 2.00, 'Team', 'Daily'),
(5, 'TotalLinesSuggested', 'Multiplier', 1.00, 'Team', 'Daily'),
(6, 'TotalLinesAccepted', 'Multiplier', 2.00, 'Team', 'Daily'),
(7, 'TotalActiveUsers', 'Multiplier', 5.00, 'Team', 'Daily'),
(8, 'TotalChatAcceptances', 'Multiplier', 2.50, 'Team', 'Daily'),
(9, 'TotalChatTurns', 'Multiplier', 1.00, 'Team', 'Daily'),
(10, 'TotalActiveChatUsers', 'Multiplier', 4.00, 'Team', 'Daily'),
(11, 'TakeUserSatisfactionSurvey', 'Fixed', 1000.00, 'Individual', 'Weekly')
ON CONFLICT (ActivityId) DO NOTHING;
EOF

echo "Executing SQL script..."

# timeout 30 sec
timeout 30 psql "$CONN_STR" -f "$SQL_FILE"

rm -f "$SQL_FILE"
echo "Database setup completed successfully."
