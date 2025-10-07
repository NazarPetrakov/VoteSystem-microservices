export interface Poll {
  id: string;
  title: string;
  description: string;
  createdByUserId: number;
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
