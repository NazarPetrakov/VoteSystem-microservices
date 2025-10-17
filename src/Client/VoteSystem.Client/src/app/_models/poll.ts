export interface Poll {
  id: string;
  title: string;
  description: string;
  userId: number;
  userName: string;
  isClosed: boolean;
  createdTime: Date;
  updatedTime: Date;
  pollOptions: PollOption[];
}
export interface PollOption {
  id: string;
  text: string;
  order: number;
  pollId: string;
  createdTime: Date;
  updatedTime?: Date;
}
