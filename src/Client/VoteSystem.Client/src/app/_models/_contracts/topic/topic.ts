export interface Topic {
  value: string;
  viewValue: string;
}

export interface TopicGroup {
  name: string;
  topics: Topic[];
}
export const TopicGroups: TopicGroup[] = [
  {
    name: 'General / Fun',
    topics: [
      { value: 'daily-life', viewValue: 'Daily Life' },
      { value: 'entertainment', viewValue: 'Entertainment' },
      { value: 'movies-tv-shows', viewValue: 'Movies & TV Shows' },
      { value: 'music', viewValue: 'Music' },
      { value: 'books-literature', viewValue: 'Books & Literature' },
      { value: 'food-drink', viewValue: 'Food & Drink' },
      { value: 'travel-places', viewValue: 'Travel & Places' },
      { value: 'hobbies', viewValue: 'Hobbies' },
    ],
  },
  {
    name: 'Work / Professional',
    topics: [
      { value: 'company-decisions', viewValue: 'Company Decisions' },
      { value: 'team-feedback', viewValue: 'Team Feedback' },
      { value: 'project-priorities', viewValue: 'Project Priorities' },
      { value: 'workplace-satisfaction', viewValue: 'Workplace Satisfaction' },
      {
        value: 'training-skill-assessment',
        viewValue: 'Training / Skill Assessment',
      },
    ],
  },
  {
    name: 'Social / Opinion',
    topics: [
      { value: 'politics', viewValue: 'Politics' },
      { value: 'society-culture', viewValue: 'Society & Culture' },
      { value: 'technology-innovation', viewValue: 'Technology & Innovation' },
      {
        value: 'environment-sustainability',
        viewValue: 'Environment & Sustainability',
      },
      { value: 'education', viewValue: 'Education' },
    ],
  },
  {
    name: 'Events / Seasonal',
    topics: [
      { value: 'holidays', viewValue: 'Holidays' },
      {
        value: 'birthday-celebration',
        viewValue: 'Birthday / Celebration Polls',
      },
      { value: 'sports-events', viewValue: 'Sports Events' },
      { value: 'conferences-meetups', viewValue: 'Conferences & Meetups' },
    ],
  },
  {
    name: 'Personal / Small Groups',
    topics: [
      { value: 'friends-family', viewValue: 'Friends & Family' },
      { value: 'community-decisions', viewValue: 'Community Decisions' },
      {
        value: 'group-trips-activities',
        viewValue: 'Group Trips / Activities',
      },
      {
        value: 'game-nights-competitions',
        viewValue: 'Game Nights / Competitions',
      },
    ],
  },
  {
    name: 'Misc / Interactive',
    topics: [
      { value: 'quizzes-trivia', viewValue: 'Quizzes & Trivia' },
      { value: 'prediction-polls', viewValue: 'Prediction Polls' },
      {
        value: 'ratings',
        viewValue: 'Ratings (like “best product / movie / book”)',
      },
      { value: 'contests-competitions', viewValue: 'Contests / Competitions' },
    ],
  },
];
